using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.Workorders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks
{
    internal class UpdateWorkOrderRepairTasksCommandHandler(IAppDbContext context,
        ILogger<UpdateWorkOrderRepairTasksCommandHandler> logger,
        IWorkOrderPolicy workOrderPolicy,HybridCache cache)
        :IRequestHandler<UpdateWorkOrderRepairTasksCommand,Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateWorkOrderRepairTasksCommandHandler> _logger = logger;
        private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Updated>> Handle(UpdateWorkOrderRepairTasksCommand request, CancellationToken ct)
        {
            var workOrder = await _context.Workorders
                            .Include(w => w.RepairTasks)
                    .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, ct);

            if (workOrder is null)
            {
                logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            if (request.RepairTasksIds.Length == 0)
            {
                logger.LogError("Empty RepairTaskIds list submitted.");

                return RepairTaskErrors.AtLeastOneRepairTaskIsRequired;
            }

            var requestedTasks = await context.RepairTasks
                .Where(t => request.RepairTasksIds.Contains(t.Id))
                .ToListAsync(ct);

            if (requestedTasks.Count != request.RepairTasksIds.Length)
            {
                var missingIds = request.RepairTasksIds.Except(requestedTasks.Select(t => t.Id)).ToArray();

                logger.LogError("One or more RepairTasks not found. {ids}", string.Join(", ", missingIds));

                return ApplicationErrors.RepairTaskNotFound;
            }


            var clearExistingResult = workOrder.ClearRepairTasks();

            if (clearExistingResult.IsError)
            {
                return clearExistingResult;
            }

            foreach (var task in requestedTasks)
            {
                var addRepairTaskResult = workOrder.AddRepairTask(task);

                if (addRepairTaskResult.IsError)
                {
                    return addRepairTaskResult;
                }
            }

            var totalDuration = TimeSpan.FromMinutes(requestedTasks.Sum(x => (int)x.EstimatedDurationInMins));

            var newEndAt = workOrder.StartAtUtc + totalDuration;

            // Business validations
            if (_workOrderPolicy.IsOutsideOperatingHours(workOrder.StartAtUtc, totalDuration))
            {
                return Error.Conflict("WorkOrder_Outside_OperatingHours", "WorkOrder timing exceeds business hours.");
            }

            var spotCheckResult = await _workOrderPolicy.CheckSpotAvailabilityAsync(
                workOrder.Spot,
                workOrder.StartAtUtc,
                newEndAt,
                excludeWorkOrderId: workOrder.Id,
                ct: ct);

            if (spotCheckResult.IsError)
            {
                return spotCheckResult.Errors;
            }

            if (await _workOrderPolicy.IsLaborOccupied(workOrder.LaborId, workOrder.Id, workOrder.StartAtUtc, newEndAt))
            {
                return ApplicationErrors.LaborOccupied;
            }

            workOrder.UpdateTiming(workOrder.StartAtUtc, newEndAt);

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await context.SaveChangesAsync(ct);

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await cache.RemoveByTagAsync("work-order", ct);

            return Result.Updated;
        }
    }
}


using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder
{
    public class RelocateWorkOrderCommandHandler(IAppDbContext context,
        ILogger<RelocateWorkOrderCommandHandler> logger,
        HybridCache cache,
        IWorkOrderPolicy workOrderPolicy) 
        : IRequestHandler<RelocateWorkOrderCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<RelocateWorkOrderCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;

        public async Task<Result<Updated>> Handle(RelocateWorkOrderCommand request, CancellationToken ct)
        {
            var workOrder = await _context.Workorders
                .Include(w => w.Vehicle)
                .Include(w => w.Labor)
                .Include(w => w.RepairTasks)
                .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, ct);

            if (workOrder is null)
            {
                _logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }

            var duration = workOrder.EndAtUtc.Subtract(workOrder.StartAtUtc).Duration();

            var endAt = request.NewStartAt.Add(duration);

            var checkSpotAvailabilityResult = await _workOrderPolicy.CheckSpotAvailabilityAsync(
                workOrder.Spot,
                request.NewStartAt,
                endAt,
                excludeWorkOrderId: workOrder.Id,
                ct);

            if (checkSpotAvailabilityResult.IsError)
            {
                _logger.LogError("Spot: {Spot} is not available.", workOrder.Spot.ToString());

                return checkSpotAvailabilityResult.Errors!;
            }

            if(await _workOrderPolicy.IsLaborOccupied(workOrder.LaborId, workOrder.Id, request.NewStartAt, endAt))
            {
                _logger.LogError("Labor with Id '{LaborId}' is occupied.", workOrder.LaborId);
                return ApplicationErrors.LaborOccupied;
            }

            if (await _workOrderPolicy.IsVehicleAlreadyScheduled(workOrder.VehicleId, request.NewStartAt, endAt, request.WorkOrderId))
            {
                _logger.LogError("Vehicle with Id '{VehicleId}' already has an overlapping WorkOrder.", workOrder.VehicleId);

                return ApplicationErrors.VehicleSchedulingConflict;
            }

            var updateTimingResult = workOrder.UpdateTiming(request.NewStartAt, endAt);

            if (updateTimingResult.IsError)
            {
                _logger.LogError("Failed to update timing: {Error}", updateTimingResult.TopError.Description);

                return updateTimingResult.Errors!;
            }

            var updateSpotResult = workOrder.UpdateSpot(request.NewSpot);

            if (updateTimingResult.IsError)
            {
                _logger.LogError("Failed to update Spot: {Error}", updateSpotResult.TopError.Description);

                return updateTimingResult.Errors!;
            }


            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await _context.SaveChangesAsync(ct);

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await _cache.RemoveByTagAsync("work-order", ct);

            return Result.Updated;
        }
    }
}

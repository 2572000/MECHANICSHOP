using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders;
using MechanicShop.Domain.Workorders.Enums;
using MechanicShop.Domain.Workorders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState
{
    public class UpdateWorkOrderStateCommandHandler(IAppDbContext context,
        ILogger<UpdateWorkOrderStateCommandHandler> logger,
        HybridCache cache,TimeProvider timeProvider) 
        : IRequestHandler<UpdateWorkOrderStateCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateWorkOrderStateCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<Result<Updated>> Handle(UpdateWorkOrderStateCommand request, CancellationToken ct)
        {
            var workOrder = await _context.Workorders
                        .FindAsync([request.WorkOrderId], ct);

            if (workOrder is null)
            {
                _logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", request.WorkOrderId);

                return ApplicationErrors.WorkOrderNotFound;
            }

            if (workOrder.StartAtUtc > _timeProvider.GetUtcNow())
            {
                _logger.LogError("State transition for WorkOrder Id '{WorkOrderId}` is not allowed before the work order�s scheduled start time.", request.WorkOrderId);

                return WorkorderErrors.StateTransitionNotAllowed(workOrder.StartAtUtc);
            }
            var updateStatusResult = workOrder.UpdateState(request.NewState);

            if (updateStatusResult.IsError)
            {
                _logger.LogError("Failed to update status: {Error}", updateStatusResult.TopError.Description);

                return updateStatusResult.Errors!;
            }

            if (request.NewState == WorkOrderState.Completed)
            {
                workOrder.AddDomainEvent(new WorkOrderCompleted { WorkOrderId = request.WorkOrderId });
            }

            await _context.SaveChangesAsync(ct);

            workOrder.AddDomainEvent(new WorkOrderCollectionModified());

            await _cache.RemoveByTagAsync("work-order", ct);

            return Result.Updated;
        }
    }
}

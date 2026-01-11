using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders;
using MechanicShop.Domain.Workorders.Enums;
using MechanicShop.Domain.Workorders.Events;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder
{
    public class DeleteWorkOrderCommandHandler(IAppDbContext context,
        ILogger<DeleteWorkOrderCommandHandler> logger,
        HybridCache cache)
        :IRequestHandler<DeleteWorkOrderCommand,Result<Deleted>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<DeleteWorkOrderCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Deleted>> Handle(DeleteWorkOrderCommand request, CancellationToken ct)
        {
            var workOrder = await _context.Workorders.FindAsync([request.WorkOrderId], ct);
            if (workOrder == null)
            {
                _logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }

            if(workOrder.State is not WorkOrderState.Scheduled)
            {
                _logger.LogError(
              "Deletion failed: only 'Scheduled' or 'Confirmed' WorkOrders can be deleted. Current status: {Status}",
              workOrder.State);

                return WorkorderErrors.Readonly;
            }
            _context.Workorders.Remove(workOrder);
            await _context.SaveChangesAsync(ct);
            workOrder.AddDomainEvent(new WorkOrderCollectionModified());
            await _cache.RemoveByTagAsync("work-order", ct);

            return Result.Deleted;


        }
    }
}

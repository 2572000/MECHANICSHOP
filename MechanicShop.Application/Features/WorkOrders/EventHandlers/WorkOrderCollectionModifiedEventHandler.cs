using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Workorders.Events;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.EventHandlers
{
    internal class WorkOrderCollectionModifiedEventHandler(IWorkOrderNotifier workOrderNotifier)
        :INotificationHandler<WorkOrderCollectionModified>
    {
        private readonly IWorkOrderNotifier _workOrderNotifier = workOrderNotifier;

        public Task Handle(WorkOrderCollectionModified notification, CancellationToken ct)=>
            _workOrderNotifier.NotifyWorkOrdersChangedAsync(ct);
        
    }
}

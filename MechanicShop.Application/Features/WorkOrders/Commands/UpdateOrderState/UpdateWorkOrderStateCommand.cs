using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders.Enums;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState
{
    public record UpdateWorkOrderStateCommand(Guid WorkOrderId, WorkOrderState NewState) 
        : IRequest<Result<Updated>>;
}

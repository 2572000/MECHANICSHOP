using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder
{
    public record DeleteWorkOrderCommand(Guid WorkOrderId):IRequest<Result<Deleted>>;
    
}

using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask
{
    public record RemoveRepairTaskCommand(Guid RepairTaskId)
        : IRequest<Result<Deleted>>;
    
}

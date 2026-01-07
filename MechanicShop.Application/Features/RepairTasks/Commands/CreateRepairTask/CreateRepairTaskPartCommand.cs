using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public record CreateRepairTaskPartCommand(
        string? Name,
        int Quantity,
        decimal Cost) : IRequest<Result<Success>>;
}
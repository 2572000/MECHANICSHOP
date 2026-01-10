namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public record UpdateRepairTaskPartCommand(
        Guid? PartId,
        string Name,
        decimal Cost,
        int Quantity
        )
    {
    }
}
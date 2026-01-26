namespace MechanicShop.Contracts.Request.WorkOrder
{
    public class ModifyRepairTaskRequest
    {
        public Guid[] RepairTaskIds { get; set; } = [];
    }
}

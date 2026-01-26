namespace MechanicShop.Contracts.Request.WorkOrder
{
    public record PageRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

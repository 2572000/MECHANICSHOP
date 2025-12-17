using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks.Parts
{
    public sealed class Part:AuditableEntity
    {
        public string? Name { get;private set; }
        public int Quantity { get; private set; }
        public decimal Cost { get; private set; }

        private Part()
        {
            
        }

        private Part(Guid id,string name,int quantity,decimal cost)
            :base(id)
        {
            Name = name;
            Quantity = quantity;
            Cost = cost;
        }

        public static Result<Part> Create(Guid id,string name,int quantity,decimal cost)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return PartErrors.NameRequired;
            }
            if(quantity <= 0 || quantity > 10)
            {
                return PartErrors.QuantityInvalid;
            }
            if(cost <= 0 || cost > 10000)
            {
                return PartErrors.CostInvalid;
            }
            return new Part(id,name.Trim(),quantity,cost);
        }

        public Result<Updated> Update(string name,int quantity,decimal cost)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return PartErrors.NameRequired;
            }
            if(quantity <= 0 || quantity > 10)
            {
                return PartErrors.QuantityInvalid;
            }
            if(cost <= 0 || cost > 10000)
            {
                return PartErrors.CostInvalid;
            }
            Name = name.Trim();
            Quantity = quantity;
            Cost = cost;
            return Result.Updated;
        }
    }
}

using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.RepairTasks.Parts
{
    public static class PartErrors
    {
        public static Error NameRequired =>
            Error.Validation("Part.Name.Required", "Part name is required.");

        public static Error QuantityInvalid =>
            Error.Validation("Part.Quantity.Invalid", "Part quantity must be between 1 and 10.");
        public static Error CostInvalid =>
            Error.Validation("Part.Cost.Invalid", "Part cost must be between 0.01 and 10,000.");
    }
}

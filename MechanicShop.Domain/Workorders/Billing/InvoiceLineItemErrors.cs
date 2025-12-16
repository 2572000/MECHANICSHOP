using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Workorders.Billing
{
    public static class InvoiceLineItemErrors
    {
        public static Error InvoiceIdRequired=>
            Error.Validation("InvoiceLineItem.InvoiceId.Required","Invoice ID is required.");

        public static Error LineNumberInvalid =>
            Error.Validation("InvoiceLineItem.LineNumber.Invalid", "Line number must be greater than zero.");

        public static Error DescriptionRequired =>
            Error.Validation("InvoiceLineItem.Description.Required", "Description is required.");
        public static Error QuantityInvalid =>
            Error.Validation("InvoiceLineItem.Quantity.Invalid", "Quantity must be greater than zero.");
        public static Error UnitPriceInvalid =>
            Error.Validation("InvoiceLineItem.UnitPrice.Invalid", "Unit price must be greater than zero.");
    }
}

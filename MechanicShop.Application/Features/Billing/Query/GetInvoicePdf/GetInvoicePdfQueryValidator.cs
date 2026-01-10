using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Query.GetInvoicePdf
{
    public class GetInvoicePdfQueryValidator:AbstractValidator<GetInvoicePdfQuery>
    {
        public GetInvoicePdfQueryValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithErrorCode("InvoiceId_Is_Required")
                .WithMessage("Invoice Id Is Required");
        }
    }
}

using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Query.GetInvoiceById
{
    public class GetInvoiceByIdQueryValidator:AbstractValidator<GetInvoiceByIdQuery>
    {
        public GetInvoiceByIdQueryValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithErrorCode("InvoiceId_Is_Required")
                .WithMessage("InvoiceId must not be empty.");
        }
    }
}

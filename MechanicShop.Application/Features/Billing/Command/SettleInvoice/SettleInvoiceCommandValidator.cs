using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Command.SettleInvoice
{
    public class SettleInvoiceCommandValidator:AbstractValidator<SettleInvoiceCommand>
    {
        public SettleInvoiceCommandValidator()
        {
            RuleFor(x => x.InvoiceId)
                .NotEmpty()
                .WithErrorCode("InvoiceId_Is_Required")
                .WithMessage("InvoiceId must not be empty.");
        }
    }
}

using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Command.IssueInvoice
{
    public class IssueInvoiceCommandValidator:AbstractValidator<IssueInvoiceCommand>
    {
        public IssueInvoiceCommandValidator()
        {
            RuleFor(i => i.WorkOrderId)
                .NotEmpty()
                .WithErrorCode("WorkOrderId_Is_Required")
                .WithMessage("Work Order Id Is Required.");
        }
    }
}

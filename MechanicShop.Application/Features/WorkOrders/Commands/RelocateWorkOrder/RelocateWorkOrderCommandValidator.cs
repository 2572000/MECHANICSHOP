using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder
{
    public class RelocateWorkOrderCommandValidator:AbstractValidator<RelocateWorkOrderCommand>
    {
        public RelocateWorkOrderCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("WorkOrderId must not be empty.");

            RuleFor(x => x.NewStartAt)
                .GreaterThan(DateTimeOffset.UtcNow).WithMessage("NewStartAt must be in the future.");

            RuleFor(x => x.NewSpot)
                .IsInEnum().WithMessage("NewSpot must be a valid spot.");
        }
    }
}

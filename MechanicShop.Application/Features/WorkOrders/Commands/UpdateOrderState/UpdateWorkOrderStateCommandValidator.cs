using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateOrderState
{
    internal class UpdateWorkOrderStateCommandValidator:AbstractValidator<UpdateWorkOrderStateCommand>
    {
        public UpdateWorkOrderStateCommandValidator()
        {
           

            RuleFor(x => x.NewState)
                .IsInEnum()
                .WithErrorCode("WorkOrderStatus_Invalid")
                .WithMessage("New state must be a valid WorkOrderState.");
        }
    }
}

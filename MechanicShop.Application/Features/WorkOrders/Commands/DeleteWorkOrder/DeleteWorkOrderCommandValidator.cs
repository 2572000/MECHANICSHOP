using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder
{
    public class DeleteWorkOrderCommandValidator:AbstractValidator<DeleteWorkOrderCommand>
    {
        public DeleteWorkOrderCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty()
                .WithErrorCode("WorkOrderId_Is_Required")
                .WithMessage("WorkOrderId Is Required."); 
        }
    }
}

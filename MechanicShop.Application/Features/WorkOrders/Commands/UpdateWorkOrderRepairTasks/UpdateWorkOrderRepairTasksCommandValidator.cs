using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderRepairTasks
{
    internal class UpdateWorkOrderRepairTasksCommandValidator:AbstractValidator<UpdateWorkOrderRepairTasksCommand>
    {
        public UpdateWorkOrderRepairTasksCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .NotEmpty().WithMessage("Work order ID must not be empty.");

            RuleFor(x => x.RepairTasksIds)
                .NotNull().WithMessage("Repair tasks IDs must not be null.")
                .Must(ids => ids.Length > 0).WithMessage("At least one repair task ID must be provided.");
        }
    }
}

using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public class UpdateRepairTaskCommandValidator:AbstractValidator<UpdateRepairTaskCommand>
    {
        public UpdateRepairTaskCommandValidator()
        {
            RuleFor(x => x.RepairTaskId)
                .NotEmpty().WithMessage("Repair task ID is required.");

            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Repair Task Name Is Required")
                .MaximumLength(100);

            RuleFor(r => r.LaborCost)
                .InclusiveBetween(1, 10_00)
                .WithMessage("Cost must be between 1 and 10,000.");

            RuleFor(r=>r.EstimatedDurationInMins)
                .IsInEnum()
                .WithMessage("Invalid duration selected.");

            RuleFor(r => r.Parts)
                .NotNull()
                .Must(p => p.Count > 0)
                .WithMessage("At lest One Part Is Required");

            RuleForEach(r=>r.Parts)
                .SetValidator(new UpdateRepairTaskPartCommandValidator());
        }
    }
}

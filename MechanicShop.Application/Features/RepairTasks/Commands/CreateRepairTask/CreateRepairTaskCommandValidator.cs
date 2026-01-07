using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public class CreateRepairTaskCommandValidator :AbstractValidator<CreateRepairTaskCommand>
    {
        public CreateRepairTaskCommandValidator()
        {
            RuleFor(r=>r.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(r=>r.LaborCost)
                .GreaterThan(0).WithMessage("Labor cost must be greater than 0.");

            RuleFor(r => r.EstimatedDurationInMins)
                .NotNull().WithMessage("Estimated duration is required.")
                .IsInEnum();

            RuleFor(r => r.Parts)
                .NotNull().WithMessage("Parts list cannot be null.")
                .Must(parts => parts.Count > 0).WithMessage("At least one part is required.");

            RuleForEach(r => r.Parts).SetValidator(new CreateRepairTaskPartCommandValidator());
        }
    }
}

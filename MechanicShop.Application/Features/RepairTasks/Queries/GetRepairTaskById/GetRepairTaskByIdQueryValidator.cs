using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById
{
    public class GetRepairTaskByIdQueryValidator:AbstractValidator<GetRepairTaskByIdQuery>
    {
        public GetRepairTaskByIdQueryValidator()
        {
            RuleFor(p => p.RepairTaskId)
                .NotEmpty()
                .WithErrorCode("RepairTaskId_Is_Required")
                .WithMessage("Repair Task Id is required.");
        }
    }
}

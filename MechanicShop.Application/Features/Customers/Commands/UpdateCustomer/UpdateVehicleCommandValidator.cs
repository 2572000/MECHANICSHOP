using FluentValidation;
using FluentValidation.Validators;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleCommandValidator()
        {

            RuleFor(v=>v.Make)
                .NotEmpty().WithMessage("Make is required.")
                .MaximumLength(50).WithMessage("Make must not exceed 50 characters.");
            RuleFor(v=>v.Model)
                .NotEmpty().WithMessage("Model is required.")
                .MaximumLength(50).WithMessage("Model must not exceed 50 characters.");

            RuleFor(v=>v.LicensePlate)
                .NotEmpty().WithMessage("LicensePlate is required.")
                .MaximumLength(10).WithMessage("LicensePlate must not exceed 10 characters.");
        }
    }
}
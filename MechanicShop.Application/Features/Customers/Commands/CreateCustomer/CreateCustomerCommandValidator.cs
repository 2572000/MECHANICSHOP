using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer
{
    public sealed class CreateCustomerCommandValidator:AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name Is Required")
                .MaximumLength(100);

            RuleFor(c => c.Email)
               .NotEmpty().WithMessage("Email Is Required")
               .MaximumLength(100);

            RuleFor(c => c.PhoneNumber)
             .NotEmpty().WithMessage("Phone Number Is Required")
             .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7–15 digits and may start with '+'.");

            RuleFor(c => c.Vehicles)
                .NotNull().WithMessage("Vehicle list cannot be null.")
                .Must(v => v.Count > 0).WithMessage("At least one vehicle is required.");

            RuleForEach(c => c.Vehicles).SetValidator(new CreateVehicleCommandValidator());
        }
    }
}

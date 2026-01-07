using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandValidator:AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(c=>c.CustomerId)
                .NotEmpty();

            RuleFor(c=>c.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(c=>c.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber is required.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7–15 digits and may start with '+'.");
            
            RuleFor(c=>c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(c=>c.Vehicles)
                .NotNull().WithMessage("Vehicles list cannot be null.")
                .Must(v=>v.Count>0).WithMessage("At least one vehicle is required.");

            RuleForEach(c => c.Vehicles).SetValidator(new UpdateVehicleCommandValidator());
        }
    }
}

using FluentValidation;

namespace MechanicShop.Application.Features.Identity.Queries.GenerateTokens
{
    public class GenerateTokensQueryValidator:AbstractValidator<GenerateTokensQuery>
    {
        public GenerateTokensQueryValidator()
        {
            RuleFor(t => t.Email)
                .NotEmpty().NotNull()
                .WithErrorCode("Email_Null_Or_Empty")
                .WithMessage("Email cannot be null or empty");

            RuleFor(t => t.Password)
                .NotEmpty().NotNull()
                .WithErrorCode("Password_Null_Or_Empty")
                .WithMessage("Password cannot be null or empty.");
        }
    }
}

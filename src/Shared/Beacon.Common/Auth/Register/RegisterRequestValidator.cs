using FluentValidation;

namespace Beacon.Common.Auth.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.EmailAddress)
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(r => r.DisplayName)
            .NotEmpty().WithMessage("Display name is required.");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

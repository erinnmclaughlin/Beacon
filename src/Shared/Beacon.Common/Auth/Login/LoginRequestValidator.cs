using FluentValidation;

namespace Beacon.Common.Auth.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {

        RuleFor(r => r.EmailAddress)
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
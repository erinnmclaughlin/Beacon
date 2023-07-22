using FluentValidation;

namespace Beacon.Common.Requests.Auth;

[AllowAnonymous]
public sealed class LoginRequest : BeaconRequest<LoginRequest>
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public class Validator : AbstractValidator<LoginRequest>
    {
        public Validator()
        {
            RuleFor(r => r.EmailAddress)
                .EmailAddress().WithMessage("Valid email address is required.");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}

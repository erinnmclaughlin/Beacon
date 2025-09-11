using Beacon.Common.Validation.Rules;
using FluentValidation;

namespace Beacon.Common.Requests.Auth;

[AllowAnonymous]
public sealed class RegisterRequest : BeaconRequest<RegisterRequest>
{
    public string DisplayName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public class Validator : AbstractValidator<RegisterRequest>
    {
        public Validator()
        {
            RuleFor(r => r.EmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(r => r.DisplayName)
                .NotEmpty().WithMessage("Display name is required.");

            RuleFor(r => r.Password)
                .IsValidPassword();

            RuleFor(r => r.ConfirmPassword)
                .NotEmpty().WithMessage("Please confirm your password.")
                .When(r => !string.IsNullOrWhiteSpace(r.Password));

            RuleFor(r => r.ConfirmPassword)
                .Equal(r => r.Password)
                .WithMessage("Passwords do not match.")
                .When(r => !string.IsNullOrWhiteSpace(r.ConfirmPassword));
        }
    }
}

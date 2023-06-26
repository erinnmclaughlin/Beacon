using FluentValidation;
using MediatR;

namespace Beacon.Common.Requests.Auth;

public sealed class LoginRequest : IRequest
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

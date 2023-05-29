using FluentValidation;

namespace Beacon.Common.Auth.Requests;

public class LoginRequest : IApiRequest<UserDto>
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public class Validator : AbstractValidator<LoginRequest>
    {
        public Validator()
        {

            RuleFor(r => r.EmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}

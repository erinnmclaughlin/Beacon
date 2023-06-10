using FluentValidation;

namespace Beacon.Common.Validation.Rules;

public static class AuthRules
{
    public const int MinimumPasswordLength = 8;

    public static IRuleBuilderOptions<T, string> IsValidPassword<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(MinimumPasswordLength).WithMessage($"Password must contain at least {MinimumPasswordLength} characters.");
    }
}

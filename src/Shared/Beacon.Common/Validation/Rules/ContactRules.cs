using FluentValidation;

namespace Beacon.Common.Validation.Rules;

public static class ContactRules
{
    public const int MinimumNameLength = 3;
    public const int MaximumNameLength = 100;
    public static IRuleBuilderOptions<T, string> IsValidContactName<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Contact name is required.")
            .MinimumLength(MinimumNameLength).WithMessage($"Contact name must be at least {MinimumNameLength} characters.")
            .MaximumLength(MaximumNameLength).WithMessage($"Contact name cannot exceed {MaximumNameLength} characters.");
    }

    public const int MinimumPhoneNumberLength = 7;
    public const int MaximumPhoneNumberLength = 20;
    public static IRuleBuilderOptions<T, string?> IsValidPhoneNumber<T>(this IRuleBuilder<T, string?> builder)
    {
        return builder
            .MinimumLength(MinimumPhoneNumberLength).WithMessage("Phone number is invalid.")
            .MaximumLength(MaximumPhoneNumberLength).WithMessage($"Phone number cannot exceed {MaximumPhoneNumberLength} characters.");
    }
}

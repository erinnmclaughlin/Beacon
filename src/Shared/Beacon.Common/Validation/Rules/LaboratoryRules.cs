using FluentValidation;

namespace Beacon.Common.Validation.Rules;

public static class LaboratoryRules
{
    public const int MinimumNameLength = 3;
    public const int MaximumNameLength = 50;

    public static IRuleBuilderOptions<T, string> IsValidLaboratoryName<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Laboratory name is required.")
            .MinimumLength(MinimumNameLength).WithMessage($"Laboratory name must contain at least {MinimumNameLength} characters.")
            .MaximumLength(MaximumNameLength).WithMessage($"Laboratory name cannot exceed {MaximumNameLength} characters.");
    }
}

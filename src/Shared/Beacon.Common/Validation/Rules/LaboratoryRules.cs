using FluentValidation;

namespace Beacon.Common.Validation.Rules;

public static class LaboratoryRules
{
    public const int MinimumLaboratoryNameLength = 3;
    public const int MaximumLaboratoryNameLength = 50;

    public static IRuleBuilderOptions<T, string> IsValidLaboratoryName<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Laboratory name is required.")
            .MinimumLength(MinimumLaboratoryNameLength).WithMessage($"Laboratory name must contain at least {MinimumLaboratoryNameLength} characters.")
            .MaximumLength(MaximumLaboratoryNameLength).WithMessage($"Laboratory name cannot exceed {MaximumLaboratoryNameLength} characters.");
    }
}

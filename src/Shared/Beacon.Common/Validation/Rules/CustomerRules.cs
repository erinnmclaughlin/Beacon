using FluentValidation;

namespace Beacon.Common.Validation.Rules;

public static class CustomerRules
{
    public const int CompanyCodeLength = 3;
    public const int MinimumCompanyNameLength = 3;
    public const int MaximumCompanyNameLength = 50;

    public static IRuleBuilderOptions<T, string> IsValidCompanyCode<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Company code is required.")
            .Length(CompanyCodeLength).WithMessage($"Company code must be exactly {CompanyCodeLength} characters.");
    }

    public static IRuleBuilderOptions<T, string> IsValidCompanyName<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Comany name is required.")
            .MinimumLength(MinimumCompanyNameLength).WithMessage($"Company name must contain at least {MinimumCompanyNameLength} characters.")
            .MaximumLength(MaximumCompanyNameLength).WithMessage($"Company name cannot exceed {MaximumCompanyNameLength} characters.");
    }
}

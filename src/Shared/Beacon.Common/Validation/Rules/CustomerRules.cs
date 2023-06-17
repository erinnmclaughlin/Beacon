using FluentValidation;

namespace Beacon.Common.Validation.Rules;

public static class CustomerRules
{
    public const int CustomerCodeLength = 3;
    public const int MinimumCustomerNameLength = 3;
    public const int MaximumCustomerNameLength = 50;

    public static IRuleBuilderOptions<T, string> IsValidCustomerCode<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Customer code is required.")
            .Length(CustomerCodeLength).WithMessage($"Customer code must be exactly {CustomerCodeLength} characters.");
    }

    public static IRuleBuilderOptions<T, string> IsValidCustomerName<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty().WithMessage("Customer name is required.")
            .MinimumLength(MinimumCustomerNameLength).WithMessage($"Customer name must contain at least {MinimumCustomerNameLength} characters.")
            .MaximumLength(MaximumCustomerNameLength).WithMessage($"Customer name cannot exceed {MaximumCustomerNameLength} characters.");
    }
}

using Beacon.Common.Models;

namespace Beacon.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AllowAnonymousAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class RequireMinimumMembershipAttribute(LaboratoryMembershipType minimumMembership) : Attribute
{
    public LaboratoryMembershipType[] AllowedRoles { get; } = Enum.GetValues<LaboratoryMembershipType>()
            .Where(m => m >= minimumMembership)
            .ToArray();
}
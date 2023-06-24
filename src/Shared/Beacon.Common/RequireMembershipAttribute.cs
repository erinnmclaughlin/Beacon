using Beacon.Common.Memberships;

namespace Beacon.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class RequireMinimumMembershipAttribute : Attribute
{
    public LaboratoryMembershipType[] AllowedRoles { get; }

    public RequireMinimumMembershipAttribute(LaboratoryMembershipType minimumMembership)
    {
        AllowedRoles = Enum.GetValues<LaboratoryMembershipType>()
            .Where(m => m >= minimumMembership)
            .ToArray();
    }
}
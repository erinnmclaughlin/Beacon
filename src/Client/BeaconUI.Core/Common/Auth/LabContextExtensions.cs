using Beacon.Common.Models;
using Beacon.Common.Services;

namespace BeaconUI.Core.Common.Auth;

public static class LabContextExtensions
{
    public static bool HasMinimumRole(this ILabContext labContext, LaboratoryMembershipType membershipType)
    {
        return labContext.MembershipType >= membershipType;
    }
}

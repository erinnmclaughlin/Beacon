using Beacon.Common.Models;
using System.Security.Claims;

namespace Beacon.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(BeaconClaimTypes.UserId)?.Value;
        return Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }

    public static LaboratoryMembershipType? GetMembershipType(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(BeaconClaimTypes.MembershipType)?.Value;
        return Enum.TryParse<LaboratoryMembershipType>(value, out var type) ? type : null;
    }
}
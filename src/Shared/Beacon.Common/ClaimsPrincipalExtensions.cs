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

    public static string GetDisplayName(this ClaimsPrincipal user)
    {
        return user.FindFirst(BeaconClaimTypes.DisplayName)?.Value ?? "";
    }

    public static LaboratoryMembershipType? GetMembershipType(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(BeaconClaimTypes.MembershipType)?.Value;
        return Enum.TryParse<LaboratoryMembershipType>(value, out var type) ? type : null;
    }

    public static CurrentUserDto GetCurrentUser(this ClaimsPrincipal user)
    {
        return new CurrentUserDto
        {
            Id = user.GetUserId(),
            DisplayName = user.GetDisplayName()
        };
    }

    public static Guid GetLabId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(BeaconClaimTypes.LabId)?.Value;
        return Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }

    public static string GetLabName(this ClaimsPrincipal user)
    {
        return user.FindFirst(BeaconClaimTypes.LabName)?.Value ?? "";
    }

    public static CurrentLabDto? GetCurrentLab(this ClaimsPrincipal user)
    {
        var id = user.GetLabId();
        var membershipType = user.GetMembershipType();

        if (id == default || membershipType == null)
            return null;

        return new CurrentLabDto
        {
            Id = id,
            Name = user.GetLabName(),
            MembershipType = membershipType.Value
        };
    }
}
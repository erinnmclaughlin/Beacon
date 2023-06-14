using System.Security.Claims;

namespace Beacon.Common.Auth;

public static class BeaconClaimTypes
{
    public const string UserId = ClaimTypes.NameIdentifier;
    public const string DisplayName = ClaimTypes.Name;
    public const string Email = ClaimTypes.Email;
    public const string LabId = "LabId";
    public const string LabName = "LabName";
    public const string MembershipType = ClaimTypes.Role;

    public static void AddClaim(this ClaimsIdentity identity, string claimType, string value)
    {
        identity.AddClaim(new Claim(claimType, value));
    }
}

using System.Security.Claims;

namespace Beacon.Common.Models;

public sealed record CurrentUserDto(Guid Id, string DisplayName, LaboratoryMembershipType? MembershipType)
{
    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaims(new[]
        {
            new Claim(BeaconClaimTypes.UserId, Id.ToString()),
            new Claim(BeaconClaimTypes.DisplayName, DisplayName)
        });

        if (MembershipType != null)
            identity.AddClaim(BeaconClaimTypes.MembershipType, MembershipType.Value.ToString());

        return new ClaimsPrincipal(identity);
    }
}

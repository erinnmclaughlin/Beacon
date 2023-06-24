using System.Security.Claims;

namespace Beacon.Common.Auth;

public sealed record SessionInfoDto(Guid Id, string DisplayName)
{
    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaims(new[]
        {
            new Claim(BeaconClaimTypes.UserId, Id.ToString()),
            new Claim(BeaconClaimTypes.DisplayName, DisplayName)
        });

        return new ClaimsPrincipal(identity);
    }
}

using System.Security.Claims;

namespace Beacon.Common.Services;

public class SessionContext : ISessionContext
{
    public required CurrentUser CurrentUser { get; init; }
    public required CurrentLab? CurrentLab { get; init; }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaims(CurrentUser.GetClaims());

        if (CurrentLab != null)
        {
            identity.AddClaims(CurrentLab.GetClaims());
        }

        return new ClaimsPrincipal(identity);
    }
}

using System.Security.Claims;

namespace Beacon.App.Helpers;

public static class ClaimsPrincipalHelper
{
    public static ClaimsPrincipal CreateClaimsPrincipal(Guid userId, string authType = "AuthCookie")
    {
        var identity = new ClaimsIdentity(authType);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        return new ClaimsPrincipal(identity);
    }

    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        // TODO: throw a better exception
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("There is no current user.");

        return Guid.Parse(userId);
    }
}

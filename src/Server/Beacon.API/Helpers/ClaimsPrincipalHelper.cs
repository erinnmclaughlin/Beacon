using Beacon.API.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Helpers;

public static class ClaimsPrincipalHelper
{
    public static async Task SignInAsync(this HttpContext context, Guid userId, string authType = "AuthCookie")
    {
        var claimsPrincipal = CreateClaimsPrincipal(userId, authType);
        await context.SignInAsync(claimsPrincipal);
    }

    public static ClaimsPrincipal CreateClaimsPrincipal(Guid userId, string authType = "AuthCookie")
    {
        var identity = new ClaimsIdentity(authType);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        return new ClaimsPrincipal(identity);
    }

    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("There is no current user.");

        return Guid.Parse(userId);
    }
}

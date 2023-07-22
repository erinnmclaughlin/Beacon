using System.Security.Claims;

namespace Beacon.Common.Services;

public interface ISessionContext
{
    CurrentUser CurrentUser { get; }
    CurrentLab? CurrentLab { get; }

    public Guid UserId => CurrentUser.Id;
    public bool IsLoggedIn() => CurrentUser.Id != Guid.Empty;
}

public class SessionContext : ISessionContext
{
    public required CurrentUser CurrentUser { get; init; }
    public required CurrentLab? CurrentLab { get; init; }

    public static SessionContext? FromClaimsPrincipal(ClaimsPrincipal? principal)
    {
        if (principal?.Identity?.IsAuthenticated is not true)
            return null;

        return new SessionContext
        {
            CurrentUser = CurrentUser.FromClaimsPrincipal(principal),
            CurrentLab = CurrentLab.FromClaimsPrincipal(principal)
        };
    }

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

using System.Security.Claims;

namespace Beacon.Common.Auth;

public sealed record SessionInfoDto(SessionInfoDto.UserDto CurrentUser)
{
    public sealed record UserDto(Guid Id, string DisplayName);

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaims(new[]
        {
            new Claim(BeaconClaimTypes.UserId, CurrentUser.Id.ToString()),
            new Claim(BeaconClaimTypes.DisplayName, CurrentUser.DisplayName)
        });

        return new ClaimsPrincipal(identity);
    }

    public static SessionInfoDto? FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated is not true)
            return null;

        var userId = Guid.Parse(principal.FindFirst(BeaconClaimTypes.UserId)?.Value ?? "");
        var displayName = principal.FindFirst(BeaconClaimTypes.DisplayName)?.Value ?? "";

        return new SessionInfoDto(new UserDto(userId, displayName));
    }
}

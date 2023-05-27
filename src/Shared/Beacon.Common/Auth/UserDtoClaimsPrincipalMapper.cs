using System.Security.Claims;

namespace Beacon.Common.Auth;

public static class UserDtoClaimsPrincipalMapper
{
    public static ClaimsPrincipal ToClaimsPrincipal(this UserDto? user)
    {
        if (user is null)
            return new ClaimsPrincipal(new ClaimsIdentity());

        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.EmailAddress));

        return new ClaimsPrincipal(identity);
    }

    public static UserDto? ToUserDto(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is not { IsAuthenticated: true })
            return null;

        return new UserDto
        {
            Id = Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""),
            EmailAddress = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? "",
            DisplayName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? ""
        };
    }
}

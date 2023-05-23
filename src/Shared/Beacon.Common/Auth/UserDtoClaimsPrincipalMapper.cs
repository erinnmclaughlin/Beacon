using System.Security.Claims;

namespace Beacon.Common.Auth;

public static class UserDtoClaimsPrincipalMapper
{
    public static ClaimsPrincipal ToClaimsPrincipal(this UserDto? user)
    {
        if (user is null)
            return new ClaimsPrincipal(new ClaimsIdentity());

        var identity = new ClaimsIdentity("Cookies");

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.EmailAddress));

        return new ClaimsPrincipal(identity);
    }
}

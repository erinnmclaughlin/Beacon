using System.Security.Claims;

namespace BeaconUI.Core.Auth;

public static class UserDtoClaimsPrincipalMapper
{
    public static ClaimsPrincipal ToClaimsPrincipal(this UserDto? user)
    {
        if (user is null) 
            return new ClaimsPrincipal(new ClaimsIdentity());

        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Name, user.DisplayName));

        return new ClaimsPrincipal(identity);
    }
}

using System.Security.Claims;

namespace Beacon.App.Services;

public interface ISignInManager
{
    Task SignInAsync(ClaimsPrincipal claimsPrincipal);
    Task SignOutAsync();
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Services;

public interface ISignInManager
{
    Task SignInAsync(ClaimsPrincipal principal);
    Task SignOutAsync();
}

internal class SignInManager(IHttpContextAccessor httpContextAccessor) : ISignInManager
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task SignInAsync(ClaimsPrincipal principal)
    {
        await _httpContextAccessor.HttpContext!.SignInAsync(principal);
    }

    public async Task SignOutAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync();
    }
}

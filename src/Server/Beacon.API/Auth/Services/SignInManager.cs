using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Auth.Services;

public interface ISignInManager
{
    Task SignInAsync(ClaimsPrincipal user);
    Task SignOutAsync();
}

internal class SignInManager : ISignInManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SignInManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SignInAsync(ClaimsPrincipal user)
    {
        await GetHttpContext().SignInAsync(user);
    }

    public async Task SignOutAsync()
    {
        await GetHttpContext().SignOutAsync();
    }

    private HttpContext GetHttpContext()
    {
        return _httpContextAccessor.HttpContext
            ?? throw new Exception("An unexpected error occurred while attempting to sign in.");
    }
}

using Beacon.App.Services;
using Beacon.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Services;

internal sealed class SessionManager : ISessionManager, ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(BeaconClaimTypes.UserId);
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }

    public Task SignInAsync(Guid userId)
    {
        var principal = CreateClaimsPrincipal(userId);
        return SignInAsync(principal);
    }

    public async Task SignInAsync(ClaimsPrincipal principal)
    {
        await _httpContextAccessor.HttpContext!.SignInAsync(principal);
    }

    public Task SignOutAsync()
    {
        return _httpContextAccessor.HttpContext!.SignOutAsync();
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(Guid userId, params Claim[] additionalClaims)
    {
        var identity = new ClaimsIdentity("AuthCookie");
        identity.AddClaim(BeaconClaimTypes.UserId, userId.ToString());

        if (additionalClaims.Any())
            identity.AddClaims(additionalClaims);

        return new ClaimsPrincipal(identity);
    }
}

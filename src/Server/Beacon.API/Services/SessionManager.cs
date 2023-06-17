using Beacon.App.Services;
using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Services;

internal sealed class SessionManager : ICurrentUser, ICurrentLab, ISessionManager, ISignInManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid LabId
    {
        get
        {
            var idValue = GetValue(BeaconClaimTypes.LabId);
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }

    public LaboratoryMembershipType MembershipType
    {
        get
        {
            var value = GetValue(BeaconClaimTypes.MembershipType);
            return Enum.Parse<LaboratoryMembershipType>(value ?? "");
        }
    }

    public Guid UserId
    {
        get
        {
            var idValue = GetValue(BeaconClaimTypes.UserId);
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }

    public async Task ClearCurrentLabAsync()
    {
        var identity = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;

        if (identity is null)
            return;

        identity.TryRemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == BeaconClaimTypes.LabId));
        identity.TryRemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == BeaconClaimTypes.MembershipType));

        await SignInAsync(new ClaimsPrincipal(identity));
    }

    public async Task SetCurrentLabAsync(Guid labId, LaboratoryMembershipType membershipType)
    {
        var identity = _httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;

        if (identity is null)
            return;

        identity.TryRemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == BeaconClaimTypes.LabId));
        identity.TryRemoveClaim(identity.Claims.FirstOrDefault(c => c.Type == BeaconClaimTypes.MembershipType));
        identity.AddClaim(BeaconClaimTypes.LabId, labId.ToString());
        identity.AddClaim(BeaconClaimTypes.MembershipType, membershipType.ToString());

        await SignInAsync(new ClaimsPrincipal(identity));
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

    private string? GetValue(string claimType)
    {
        return _httpContextAccessor.HttpContext!.User.FindFirstValue(claimType);
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

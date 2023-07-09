using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Services;
using BeaconUI.Core.Common.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Common.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, ICurrentUser
{
    private readonly ApiClient _apiClient;

    private ClaimsPrincipal? ClaimsPrincipal { get; set; }

    public Guid UserId => ClaimsPrincipal?.GetUserId() ?? Guid.Empty;
    public LaboratoryMembershipType? MembershipType => ClaimsPrincipal?.GetMembershipType();

    public BeaconAuthStateProvider(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (ClaimsPrincipal == null)
        {
            var errorOrUser = await _apiClient.GetCurrentUser();
            ClaimsPrincipal = errorOrUser.Match(
                value => value.ToClaimsPrincipal(),
                error => AnonymousUser);
        }

        return new AuthenticationState(ClaimsPrincipal);
    }

    public void NotifyAuthenticationStateChanged()
    {
        ClaimsPrincipal = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}

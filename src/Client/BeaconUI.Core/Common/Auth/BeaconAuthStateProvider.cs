using Beacon.Common.Services;
using BeaconUI.Core.Common.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Common.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, ISessionContext
{
    private readonly ApiClient _apiClient;

    private bool IsExpired { get; set; } = true;
    private ClaimsPrincipal ClaimsPrincipal { get; set; } = AnonymousUser;

    public CurrentUser CurrentUser => CurrentUser.FromClaimsPrincipal(ClaimsPrincipal);
    public CurrentLab? CurrentLab => CurrentLab.FromClaimsPrincipal(ClaimsPrincipal);

    public BeaconAuthStateProvider(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (IsExpired)
        {
            var errorOrUser = await _apiClient.GetCurrentUser();

            ClaimsPrincipal = errorOrUser.IsError 
                ? AnonymousUser 
                : errorOrUser.Value.ToClaimsPrincipal();

            IsExpired = false;
        }

        return new AuthenticationState(ClaimsPrincipal);
    }

    public void NotifyAuthenticationStateChanged()
    {
        IsExpired = true;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}

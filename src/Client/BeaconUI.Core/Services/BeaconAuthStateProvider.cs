using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Services;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    private readonly ApiClient _apiClient;

    public ClaimsPrincipal? ClaimsPrincipal { get; private set; }

    public BeaconAuthStateProvider(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (ClaimsPrincipal == null)
        {
            var errorOrSessionInfo = await _apiClient.GetSessionInfo();
            ClaimsPrincipal = errorOrSessionInfo.IsError ? AnonymousUser : errorOrSessionInfo.Value.ToClaimsPrincipal();
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

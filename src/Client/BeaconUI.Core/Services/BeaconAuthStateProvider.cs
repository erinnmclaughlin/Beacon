using Beacon.Common.Auth;
using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Services;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthClient _apiClient;

    public ClaimsPrincipal? CurrentUser { get; private set; }

    public BeaconAuthStateProvider(AuthClient authClient)
    {
        _apiClient = authClient;
        _apiClient.OnLogin += HandleLogin;
        _apiClient.OnLogout += HandleLogout;
    }

    public void Dispose()
    {
        _apiClient.OnLogin -= HandleLogin;
        _apiClient.OnLogout -= HandleLogout;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (CurrentUser == null)
        {
            var result = await _apiClient.GetCurrentUserAsync();
            CurrentUser = result.IsError ? AnonymousUser : result.Value.ToClaimsPrincipal();
        }

        return new AuthenticationState(CurrentUser);
    }

    private void HandleLogin(AuthUserDto user)
    {
        UpdateCurrentUser(user);
    }

    private void HandleLogout()
    {
        UpdateCurrentUser(null);
    }

    private void UpdateCurrentUser(AuthUserDto? currentUser)
    {
        CurrentUser = currentUser.ToClaimsPrincipal();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CurrentUser)));
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}

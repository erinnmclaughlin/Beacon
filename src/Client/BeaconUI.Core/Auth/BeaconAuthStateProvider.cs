using Beacon.Common.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly BeaconAuthClient _authClient;
    private ClaimsPrincipal? _currentUser;

    public BeaconAuthStateProvider(BeaconAuthClient authClient)
    {
        _authClient = authClient;

        _authClient.OnLogin += HandleLogin;
        _authClient.OnLogout += HandleLogout;
    }

    public void Dispose()
    {
        _authClient.OnLogin -= HandleLogin;
        _authClient.OnLogout -= HandleLogout;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentUser == null)
        {
            var user = await _authClient.GetCurrentUser();
            _currentUser = user.ToClaimsPrincipal();
        }

        return new AuthenticationState(_currentUser);
    }

    private void HandleLogin(UserDto user)
    {
        _currentUser = user.ToClaimsPrincipal();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private void HandleLogout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

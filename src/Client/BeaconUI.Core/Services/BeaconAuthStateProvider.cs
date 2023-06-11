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
        _apiClient.OnLogin += HandleAuthenticationStateChanged;
        _apiClient.OnLogout += HandleAuthenticationStateChanged;
    }

    public void Dispose()
    {
        _apiClient.OnLogin -= HandleAuthenticationStateChanged;
        _apiClient.OnLogout -= HandleAuthenticationStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (CurrentUser == null)
        {
            var result = await _apiClient.GetCurrentUserAsync();
            CurrentUser = GetClaimsPrincipal(result.IsError ? null : result.Value);
        }

        return new AuthenticationState(CurrentUser);
    }

    private void HandleAuthenticationStateChanged()
    {
        CurrentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());

    private static ClaimsPrincipal GetClaimsPrincipal(AuthUserDto? user)
    {
        if (user is null)
            return AnonymousUser;

        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaims(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.EmailAddress)
        });

        return new ClaimsPrincipal(identity);
    }
}

using BeaconUI.Core.Auth.Login;
using BeaconUI.Core.Auth.Register;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthService
{
    private readonly AuthenticationStateProvider _authProvider;
    private readonly HttpClient _http;

    public BeaconAuthService(AuthenticationStateProvider authProvider, HttpClient http)
    {
        _authProvider = authProvider;
        _http = http;
    }

    public async Task Register(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);

        // TODO: deal with errors:
        if (!response.IsSuccessStatusCode)
            return;

        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);

        ((BeaconAuthStateProvider)_authProvider).UpdateCurrentUser(user.ToClaimsPrincipal());
    }

    public async Task Login(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request, ct);

        // TODO: deal with errors:
        if (!response.IsSuccessStatusCode)
            return;

        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);

        ((BeaconAuthStateProvider)_authProvider).UpdateCurrentUser(user.ToClaimsPrincipal());
    }

    public async Task Logout(CancellationToken ct = default)
    {
        await _http.PostAsync("api/auth/logout", null, ct);

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        ((BeaconAuthStateProvider)_authProvider).UpdateCurrentUser(anonymousUser);
    }
}

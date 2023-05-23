using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Beacon.Common.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using OneOf;
using System.Net;
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

    public async Task<OneOf<UserDto, ValidationProblemResponse>> Register(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);

        if (response.StatusCode is HttpStatusCode.BadRequest)
        {
            var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(cancellationToken: ct);

            if (validationProblem is not null)
                return validationProblem;
        }

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct)
            ?? throw new Exception("There was an unexpected problem deserializing the response.");

        ((BeaconAuthStateProvider)_authProvider).UpdateCurrentUser(user.ToClaimsPrincipal());

        return user;
    }

    public async Task<OneOf<UserDto, ValidationProblemResponse>> Login(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request, ct);

        if (response.StatusCode is HttpStatusCode.BadRequest)
        {
            var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(cancellationToken: ct);

            if (validationProblem is not null)
                return validationProblem;
        }

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct)
            ?? throw new Exception("There was an unexpected problem deserializing the response.");

        ((BeaconAuthStateProvider)_authProvider).UpdateCurrentUser(user.ToClaimsPrincipal());

        return user;
    }

    public async Task Logout(CancellationToken ct = default)
    {
        await _http.PostAsync("api/auth/logout", null, ct);

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        ((BeaconAuthStateProvider)_authProvider).UpdateCurrentUser(anonymousUser);
    }
}

using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Beacon.Common.Responses;
using OneOf;
using System.Net;
using System.Net.Http.Json;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthClient
{
    private readonly HttpClient _http;

    public Action<UserDto>? OnLogin;

    public BeaconAuthClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<UserDto?> GetCurrentUser(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("api/users/current", ct);

        if (response.StatusCode is HttpStatusCode.NotFound)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct);
    }

    public async Task<OneOf<UserDto, ValidationProblemResponse>> Register(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);

        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(cancellationToken: ct);

            if (validationProblem is not null)
                return validationProblem;
        }

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct)
            ?? throw new Exception("There was an unexpected problem deserializing the response.");

        OnLogin?.Invoke(user);

        return user;
    }

    public async Task<OneOf<UserDto, ValidationProblemResponse>> Login(LoginRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request, ct);

        if (response.StatusCode is HttpStatusCode.UnprocessableEntity)
        {
            var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>(cancellationToken: ct);

            if (validationProblem is not null)
                return validationProblem;
        }

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct)
            ?? throw new Exception("There was an unexpected problem deserializing the response.");

        OnLogin?.Invoke(user);

        return user;
    }

    public async Task Logout(CancellationToken ct = default)
    {
        await _http.GetAsync("/api/auth/logout", ct);
    }
}

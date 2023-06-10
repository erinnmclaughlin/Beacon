using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class AuthClient : ApiClientBase
{
    public Action<AuthUserDto>? OnLogin;
    public Action? OnLogout;

    public AuthClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public Task<ErrorOr<AuthUserDto>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        return GetAsync<AuthUserDto>("api/auth/me", ct);
    }

    public async Task<ErrorOr<AuthUserDto>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var result = await PostAsync<AuthUserDto>("api/auth/login", request, ct);

        if (!result.IsError)
            OnLogin?.Invoke(result.Value);

        return result;
    }

    public async Task<ErrorOr<AuthUserDto>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var result = await PostAsync<AuthUserDto>("api/auth/register", request, ct);

        if (!result.IsError)
            OnLogin?.Invoke(result.Value);

        return result;
    }

    public async Task<ErrorOr<Success>> LogoutAsync(CancellationToken ct = default)
    {
        var result = await GetAsync("api/auth/logout", ct);

        if (!result.IsError)
            OnLogout?.Invoke();

        return result;
    }
}

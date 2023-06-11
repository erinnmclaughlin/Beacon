using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class AuthClient : ApiClientBase
{
    public Action? OnChange;

    public AuthClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public Task<ErrorOr<AuthUserDto>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        return GetAsync<AuthUserDto>("portal/me", ct);
    }

    public async Task<ErrorOr<Success>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var result = await PostAsync("portal/login", request, ct);

        if (!result.IsError)
            OnChange?.Invoke();

        return result;
    }

    public async Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var result = await PostAsync("portal/register", request, ct);

        if (!result.IsError)
            OnChange?.Invoke();

        return result;
    }

    public async Task<ErrorOr<Success>> LogoutAsync(CancellationToken ct = default)
    {
        var result = await GetAsync("portal/logout", ct);

        if (!result.IsError)
            OnChange?.Invoke();

        return result;
    }

    public async Task<ErrorOr<Success>> CreateLaboratoryAsync(CreateLaboratoryRequest request, CancellationToken ct = default)
    {
        var result = await PostAsync("portal/laboratories", request, ct);

        if (!result.IsError)
            OnChange?.Invoke();

        return result;
    }
}

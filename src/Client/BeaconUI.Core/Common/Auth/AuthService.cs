using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using BeaconUI.Core.Common.Http;
using ErrorOr;

namespace BeaconUI.Core.Common.Auth;

internal sealed class AuthService
{
    private readonly HttpClient _apiClient;
    private readonly BeaconAuthStateProvider _authStateProvider;

    public AuthService(HttpClient apiClient, BeaconAuthStateProvider authStateProvider)
    {
        _apiClient = apiClient;
        _authStateProvider = authStateProvider;
    }

    public async Task<ErrorOr<Success>> SetCurrentLaboratory(Guid id)
    {
        var result = await _apiClient.SendAsync(new SetCurrentLaboratoryRequest { LaboratoryId = id });

        if (!result.IsError)
            _authStateProvider.NotifyAuthenticationStateChanged();

        return result;
    }

    public async Task<ErrorOr<Success>> LoginAsync(LoginRequest request)
    {
        var result = await _apiClient.SendAsync(request);

        if (!result.IsError)
            _authStateProvider.NotifyAuthenticationStateChanged();

        return result;
    }

    public async Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request)
    {
        var result = await _apiClient.SendAsync(request);

        if (!result.IsError)
            _authStateProvider.NotifyAuthenticationStateChanged();

        return result;
    }

    public async Task<ErrorOr<Success>> LogoutAsync()
    {
        var result = await _apiClient.SendAsync(new LogoutRequest());

        if (!result.IsError)
            _authStateProvider.NotifyAuthenticationStateChanged();

        return result;
    }
}

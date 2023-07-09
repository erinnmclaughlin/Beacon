using Beacon.Common.Requests.Auth;
using BeaconUI.Core.Common.Http;
using ErrorOr;

namespace BeaconUI.Core.Common.Auth;

internal sealed class AuthService
{
    private readonly ApiClient _apiClient;
    private readonly BeaconAuthStateProvider _authStateProvider;

    public AuthService(ApiClient apiClient, BeaconAuthStateProvider authStateProvider)
    {
        _apiClient = apiClient;
        _authStateProvider = authStateProvider;
    }

    public async Task<ErrorOr<Success>> LoginAsync(LoginRequest request)
    {
        var result = await _apiClient.Login(request);

        if (!result.IsError)
            _authStateProvider.NotifyAuthenticationStateChanged();

        return result;
    }

    public async Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request)
    {
        var result = await _apiClient.Register(request);

        if (!result.IsError)
            _authStateProvider.NotifyAuthenticationStateChanged();

        return result;
    }

    public async Task<ErrorOr<Success>> LogoutAsync()
    {
        var result = await _apiClient.Logout();

        if (!result.IsError)
            _authStateProvider.NotifyAuthenticationStateChanged();

        return result;
    }
}

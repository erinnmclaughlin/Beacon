using Beacon.Common.Requests;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using BeaconUI.Core.Common.Http;
using ErrorOr;

namespace BeaconUI.Core.Common.Auth;

internal sealed class AuthService
{
    private readonly IApiClient _apiClient;
    private readonly IAuthenticationStateNotifier _authStateNotifier;

    public AuthService(IApiClient apiClient, IAuthenticationStateNotifier authStateNotifier)
    {
        _apiClient = apiClient;
        _authStateNotifier = authStateNotifier;
    }

    public Task<ErrorOr<Success>> SetCurrentLaboratory(Guid id)
    {
        return SendAsync(new SetCurrentLaboratoryRequest { LaboratoryId = id });
    }

    public Task<ErrorOr<Success>> LoginAsync(LoginRequest request)
    {
        return SendAsync(request);
    }

    public Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request)
    {
        return SendAsync(request);
    }

    public Task<ErrorOr<Success>> LogoutAsync()
    {
        return SendAsync(new LogoutRequest());
    }

    private async Task<ErrorOr<Success>> SendAsync<T>(BeaconRequest<T> request) where T : BeaconRequest<T>, IBeaconRequest<T>, new()
    {
        var result = await _apiClient.SendAsync(request);

        if (!result.IsError)
            await RefreshAuthenticationState();

        return result;
    }

    private async Task RefreshAuthenticationState()
    {
        var sessionContext = await _apiClient.SendAsync(new GetSessionContextRequest());
        _authStateNotifier.TriggerAuthenticationStateChanged(sessionContext.IsError ? null : sessionContext.Value);
    }
}

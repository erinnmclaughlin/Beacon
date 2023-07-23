using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using BeaconUI.Core.Common.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Common.Auth;

public interface IAuthenticationStateNotifier
{
    void TriggerAuthenticationStateChanged(SessionContext? context);
}

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, IAuthenticationStateNotifier, ISessionContext
{
    private readonly IApiClient _apiClient;

    private bool IsExpired { get; set; } = true;
    private ClaimsPrincipal ClaimsPrincipal { get; set; } = AnonymousUser;

    public CurrentUser CurrentUser => CurrentUser.FromClaimsPrincipal(ClaimsPrincipal);
    public CurrentLab? CurrentLab => CurrentLab.FromClaimsPrincipal(ClaimsPrincipal);

    public BeaconAuthStateProvider(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (IsExpired)
        {
            var errorOrUser = await _apiClient.SendAsync(new GetSessionContextRequest());

            ClaimsPrincipal = errorOrUser.IsError 
                ? AnonymousUser 
                : errorOrUser.Value.ToClaimsPrincipal();

            IsExpired = false;
        }

        return new AuthenticationState(ClaimsPrincipal);
    }

    public void TriggerAuthenticationStateChanged(SessionContext? context)
    {
        ClaimsPrincipal = context?.ToClaimsPrincipal() ?? AnonymousUser;
        var authState = new AuthenticationState(ClaimsPrincipal);
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}

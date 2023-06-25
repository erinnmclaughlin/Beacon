using Beacon.Common;
using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core.Services;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider, ICurrentUser
{
    private readonly ApiClient _apiClient;

    private ClaimsPrincipal? ClaimsPrincipal { get; set; }

    private Guid _userId;
    public Guid UserId => ClaimsPrincipal == null ? Guid.Empty : _userId;

    public BeaconAuthStateProvider(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (ClaimsPrincipal == null)
        {
            var errorOrSessionInfo = await _apiClient.GetSessionInfo();

            if (errorOrSessionInfo.IsError)
            {
                ClaimsPrincipal = AnonymousUser;
                _userId = Guid.Empty;
            }
            else
            {
                var session = errorOrSessionInfo.Value;
                ClaimsPrincipal = session.ToClaimsPrincipal();
                _userId = session.Id;
            }
        }

        return new AuthenticationState(ClaimsPrincipal);
    }

    public void NotifyAuthenticationStateChanged()
    {
        ClaimsPrincipal = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal AnonymousUser { get; } = new ClaimsPrincipal(new ClaimsIdentity());
}

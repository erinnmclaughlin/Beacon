using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BeaconUI.Core;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    public ClaimsPrincipal CurrentUser { get; private set; } = new ClaimsPrincipal(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(CurrentUser));
    }

    public void UpdateCurrentUser(ClaimsPrincipal currentUser)
    {
        CurrentUser = currentUser;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

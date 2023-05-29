using Beacon.Common.Auth;
using Beacon.Common.Auth.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Auth;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    private readonly ISender _sender;

    public BeaconAuthStateProvider(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await _sender.Send(new GetCurrentUserRequest());
        var user = result.IsError ? null : result.Value;
        return new AuthenticationState(user.ToClaimsPrincipal());
    }

    public void RefreshState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

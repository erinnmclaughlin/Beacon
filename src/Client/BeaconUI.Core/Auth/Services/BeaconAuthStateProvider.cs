using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using MediatR;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Auth.Services;

public sealed class BeaconAuthStateProvider : AuthenticationStateProvider
{
    private readonly ISender _sender;

    public UserDto? CurrentUser { get; private set; }

    public BeaconAuthStateProvider(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (CurrentUser == null)
        {
            var result = await _sender.Send(new GetCurrentUserRequest());
            CurrentUser = result.IsError ? null : result.Value;
        }

        return new AuthenticationState(CurrentUser.ToClaimsPrincipal());
    }

    public void RefreshState()
    {
        CurrentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}

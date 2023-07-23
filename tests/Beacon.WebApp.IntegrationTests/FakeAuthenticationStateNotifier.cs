using Beacon.Common.Services;
using BeaconUI.Core.Common.Auth;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;

namespace Beacon.WebApp.IntegrationTests;

public sealed class FakeAuthenticationStateNotifier : IAuthenticationStateNotifier
{
    private readonly FakeAuthenticationStateProvider _authStateProvider;

    public FakeAuthenticationStateNotifier(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = (FakeAuthenticationStateProvider)authStateProvider;
    }

    public void TriggerAuthenticationStateChanged(SessionContext? context)
    {
        _authStateProvider.TriggerAuthenticationStateChanged(
            context?.CurrentUser.DisplayName ?? "",
            claims: context?.GetClaims());
    }
}

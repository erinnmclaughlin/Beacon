using BeaconUI.Core.Common.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace Beacon.WebApp.IntegrationTests.TestHelpers;

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

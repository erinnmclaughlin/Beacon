using Beacon.Common.Auth;
using BeaconUI.Core.Auth;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Events;

internal record LoginEvent(UserDto LoggedInUser) : INotification;

internal class LoginEventHandler : INotificationHandler<LoginEvent>
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navManager;

    public LoginEventHandler(AuthenticationStateProvider authStateProvider, NavigationManager navManager)
    {
        _authStateProvider = authStateProvider;
        _navManager = navManager;
    }

    public Task Handle(LoginEvent notification, CancellationToken cancellationToken)
    {
        ((BeaconAuthStateProvider)_authStateProvider).RefreshState();
        _navManager.NavigateTo("");

        return Task.CompletedTask;
    }
}

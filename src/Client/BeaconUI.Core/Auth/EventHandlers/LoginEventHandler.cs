using Beacon.Common.Auth.Events;
using BeaconUI.Core.Auth.Services;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Auth.EventHandlers;

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

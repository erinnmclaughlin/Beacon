using BeaconUI.Core.Auth;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BeaconUI.Core.Events;
internal sealed record LogoutEvent : INotification;

internal sealed record LogoutEventHandler : INotificationHandler<LogoutEvent>
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navManager;

    public LogoutEventHandler(AuthenticationStateProvider authStateProvider, NavigationManager navManager)
    {
        _authStateProvider = authStateProvider;
        _navManager = navManager;
    }

    public Task Handle(LogoutEvent notification, CancellationToken cancellationToken)
    {
        ((BeaconAuthStateProvider)_authStateProvider).RefreshState();
        _navManager.NavigateToLogin("login");

        return Task.CompletedTask;
    }
}
using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BeaconUI.Core.Shared.Auth;

public partial class LogoutButton
{
    [Inject] private AuthClient AuthClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public async Task Logout()
    {
        var result = await AuthClient.LogoutAsync();

        if (!result.IsError)
            NavManager.NavigateToLogin("login");
    }
}
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BeaconUI.Core.Shared.Auth;

public partial class LogoutButton
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public async Task Logout()
    {
        var result = await ApiClient.Logout();

        if (!result.IsError)
            NavigationManager.NavigateToLogin("login");
    }
}
using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Auth;

public partial class LoginPage
{
    [Inject] private AuthClient AuthClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery(Name = "redirectUri")]
    public string? RedirectUri { get; set; }

    private LoginRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await AuthClient.LoginAsync(Model);

        if (result.IsError)
        {
            formContext.AddErrors(result.Errors);
            return;
        }

        NavManager.NavigateTo(RedirectUri ?? "");
    }

    private string GetRegisterPageHref()
    {
        if (!string.IsNullOrWhiteSpace(RedirectUri))
            return $"register?redirectUri={RedirectUri}";

        return "register";
    }
}
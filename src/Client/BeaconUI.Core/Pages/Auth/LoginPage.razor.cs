using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Services;
using BeaconUI.Core.Shared.Forms;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Auth;

public partial class LoginPage
{
    [Inject] 
    private AuthService AuthService { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery(Name = "redirectUri")]
    public string? RedirectUri { get; set; }

    private LoginRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await AuthService.LoginAsync(Model);

        if (result.IsError)
        {
            formContext.AddErrors(result.Errors);
            return;
        }

        NavigationManager.NavigateTo(RedirectUri ?? "");
    }

    private string GetRegisterPageHref()
    {
        if (!string.IsNullOrWhiteSpace(RedirectUri))
            return $"register?redirectUri={RedirectUri}";

        return "register";
    }
}
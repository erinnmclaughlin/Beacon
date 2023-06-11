using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Auth;

public partial class RegisterPage
{
    [Inject] private AuthClient AuthClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery(Name = "redirectUri")]
    public string? RedirectUri { get; set; }

    private RegisterRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await AuthClient.RegisterAsync(Model);

        if (result.IsError)
        {
            formContext.AddErrors(result.Errors);
            return;
        }

        NavManager.NavigateTo(RedirectUri ?? "");
    }

    private void DoAfterUpdateEmail()
    {
        if (string.IsNullOrWhiteSpace(Model.DisplayName))
            Model.DisplayName = Model.EmailAddress[..Model.EmailAddress.IndexOf('@')];
    }
}
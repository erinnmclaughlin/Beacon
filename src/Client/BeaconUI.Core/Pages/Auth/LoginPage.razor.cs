using Beacon.Common.Auth.Login;
using BeaconUI.Core.Shared;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Auth;

public partial class LoginPage
{
    [Inject] private ISender Sender { get; set; } = null!;

    private LoginRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await Sender.Send(Model);

        if (result.IsError)
            formContext.AddErrors(result.Errors);
    }
}
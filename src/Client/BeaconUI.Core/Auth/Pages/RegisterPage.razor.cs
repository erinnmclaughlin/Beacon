using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Shared;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Auth.Pages;

public partial class RegisterPage
{
    [Inject] private ISender Sender { get; set; } = null!;

    private RegisterRequest Model { get; set; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await Sender.Send(Model);

        if (result.IsError)
            formContext.AddErrors(result.Errors);
    }

    private void DoAfterUpdateEmail()
    {
        if (string.IsNullOrWhiteSpace(Model.DisplayName))
            Model.DisplayName = Model.EmailAddress[..Model.EmailAddress.IndexOf('@')];
    }
}
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class CreateLaboratoryPage
{
    private CreateLaboratoryRequest Model { get; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await ApiClient.CreateLaboratory(Model);

        if (result.IsError)
            formContext.AddErrors(result.Errors);

        NavigationManager.NavigateTo("");
    }
}
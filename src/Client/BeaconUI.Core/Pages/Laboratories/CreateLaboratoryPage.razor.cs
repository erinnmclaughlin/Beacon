using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class CreateLaboratoryPage
{
    [Inject] private AuthClient LabClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    private CreateLaboratoryRequest Model { get; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await LabClient.CreateLaboratoryAsync(Model);

        if (result.IsError)
            formContext.AddErrors(result.Errors);

        NavManager.NavigateTo("");
    }
}
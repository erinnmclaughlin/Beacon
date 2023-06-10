using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Helpers;
using BeaconUI.Core.Shared.Forms;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class CreateLaboratoryPage
{
    [Inject] private LabClient LabClient { get; set; } = null!;
    [Inject] private NavigationManager NavManager { get; set; } = null!;

    private CreateLaboratoryRequest Model { get; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        var result = await LabClient.CreateLaboratoryAsync(Model);
        result.Switch(NavigateToLabDetailsPage, formContext.AddErrors);
    }

    private void NavigateToLabDetailsPage(LaboratorySummaryDto lab)
    {
        NavManager.NavigateTo(NavigationHelper.GetLabDetailsHref(lab.Id));
    }
}
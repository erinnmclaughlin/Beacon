using Beacon.Common.Requests.Laboratories;
using BeaconUI.Core.Common.Forms;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Laboratories.Modals;

public partial class CreateLaboratoryModal
{
    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = null!;

    private CreateLaboratoryRequest Model { get; } = new();

    private async Task Submit(BeaconForm formContext)
    {
        await ApiClient.SendAsync(Model);
        await Modal.CloseAsync(ModalResult.Ok());
    }
}
using BeaconUI.Core.Laboratories.Modals;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Laboratories.Components;

public partial class CreateLaboratoryButton
{
    [CascadingParameter]
    public required IModalService ModalService { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? AdditionalAttributes { get; set; }

    [Parameter]
    public EventCallback OnLaboratoryCreated { get; set; }

    private async Task ShowCreateLabModal()
    {
        var result = await ModalService.Show<CreateLaboratoryModal>("Create New Lab").Result;
        if (result.Confirmed)
            await OnLaboratoryCreated.InvokeAsync();
    }
}
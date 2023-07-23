using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Instruments.CreateInstrument;

public partial class CreateInstrumentButton
{
    [CascadingParameter]
    public required IModalService ModalService { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? AdditionalAttributes { get; set; }

    [Parameter]
    public EventCallback OnInstrumentCreated { get; set; }

    private async Task ShowCreateInstrumentModal()
    {
        var result = await ModalService.Show<CreateInstrumentModal>("Create Instrument").Result;

        if (result.Confirmed)
            await OnInstrumentCreated.InvokeAsync();
    }
}

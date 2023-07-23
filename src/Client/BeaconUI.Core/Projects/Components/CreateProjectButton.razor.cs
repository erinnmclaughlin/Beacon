using BeaconUI.Core.Projects.Modals;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Components;

public partial class CreateProjectButton
{
    [CascadingParameter]
    private IModalService ModalService { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? AdditionalAttributes { get; set; }

    [Parameter]
    public EventCallback OnProjectCreated { get; set; }

    private async Task Click()
    {
        var result = await ModalService.Show<CreateProjectModal>("Create New Project").Result;

        if (!result.Cancelled)
        {
            await OnProjectCreated.InvokeAsync();
        }
    }
}
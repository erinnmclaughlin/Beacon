using Beacon.Common.Models;
using Beacon.Common.Services;
using BeaconUI.Core.Projects.Modals;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Components;

public partial class CreateProjectButton
{
    [CascadingParameter]
    private ILabContext LabContext { get; set; } = default!;

    [CascadingParameter]
    private IModalService ModalService { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? AdditionalAttributes { get; set; }

    [Parameter]
    public EventCallback OnProjectCreated { get; set; }

    private bool IsDisabled => LabContext.CurrentLab.MembershipType <= LaboratoryMembershipType.Analyst;

    private async Task Click()
    {
        var result = await ModalService.Show<CreateProjectModal>("Create New Project").Result;

        if (!result.Cancelled)
        {
            await OnProjectCreated.InvokeAsync();
        }
    }
}
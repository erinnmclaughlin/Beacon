using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.SampleGroups;
using BeaconUI.Core.Common.Http;
using BeaconUI.Core.Projects.Modals;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects;

public partial class ProjectSamples
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [CascadingParameter]
    private IModalService ModalService { get; set; } = default!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    private ErrorOr<SampleGroupDto[]>? SampleGroups { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadSampleGroups();
    }

    private async Task LoadSampleGroups()
    {
        SampleGroups = await ApiClient.SendAsync(new GetSampleGroupsByProjectIdRequest { ProjectId = ProjectId });
    }

    private async Task ShowAddSampleGroupModal()
    {
        var parameters = new ModalParameters().Add(nameof(AddContactModal.ProjectId), ProjectId);
        var result = await ModalService.Show<AddSampleGroupModal>("Add Sample Group", parameters).Result;
        
        if (result.Confirmed)
            await LoadSampleGroups();
    }
}
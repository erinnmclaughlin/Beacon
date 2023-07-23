using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.SampleGroups;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects;

public partial class ProjectSamples
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    private ErrorOr<SampleGroupDto[]>? SampleGroups { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadSampleGroups();
    }

    private async Task LoadSampleGroups()
    {
        SampleGroups = await ApiClient.SendAsync(new GetSampleGroupsByProjectIdRequest 
        {
            ProjectId = ProjectId
        });
    }
}
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using BeaconUI.Core.Common.Http;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects;

public partial class ProjectOverview
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [Parameter]
    public required ProjectDto Project { get; set; }

    [Parameter]
    public required EventCallback<ProjectDto> ProjectChanged { get; set; }

    private bool IsBusy { get; set; }

    private async Task UpdateLeadAnalyst(ProjectDto.LeadAnalystDto? analyst)
    {
        if (IsBusy) return;
        IsBusy = true;

        var result = await ApiClient.SendAsync(new UpdateLeadAnalystRequest { AnalystId = analyst?.Id, ProjectId = Project.Id });
        
        if (!result.IsError)
        {
            await ProjectChanged.InvokeAsync(Project with { LeadAnalyst = analyst });
        }

        IsBusy = false;
    }
}
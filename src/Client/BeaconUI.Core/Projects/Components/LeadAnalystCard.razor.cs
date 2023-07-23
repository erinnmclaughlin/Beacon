using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using BeaconUI.Core.Common.Http;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Components;

public partial class LeadAnalystCard
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [Parameter]
    public required ProjectDto Project { get; set; }

    [Parameter]
    public EventCallback<ProjectDto> ProjectChanged { get; set; }

    private bool IsBusy { get; set; }

    private async Task UpdateLeadAnalyst(LaboratoryMemberDto? member)
    {
        if (IsBusy) return;
        IsBusy = true;

        if (member is null && Project.LeadAnalyst is null)
            return;

        var result = await ApiClient.SendAsync(new UpdateLeadAnalystRequest
        {
            AnalystId = member?.Id,
            ProjectId = Project.Id 
        });

        if (!result.IsError)
        {
            await ProjectChanged.InvokeAsync(Project with
            { 
                LeadAnalyst = member is null ? null : new()
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName
                } 
            });
        }

        IsBusy = false;
    }
}
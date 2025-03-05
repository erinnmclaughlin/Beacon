using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Requests.Projects;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Components;

public partial class LeadAnalystCard
{
    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    [Parameter]
    public required ProjectDto Project { get; set; }

    [Parameter]
    public EventCallback<ProjectDto> ProjectChanged { get; set; }

    private bool IsBusy { get; set; }
    private ErrorOr<LaboratoryMemberDto[]>? ErrorOrMembers { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ErrorOrMembers = await ApiClient.SendAsync(new GetAnalystsRequest
        {
            IncludeHistoricAnalysts = false
        });
    }

    private async Task UpdateLeadAnalyst(LaboratoryMemberDto member)
    {
        if (IsBusy) return;
        IsBusy = true;

        var request = new UpdateLeadAnalystRequest
        {
            AnalystId = Project.LeadAnalyst?.Id == member.Id ? null : member.Id,
            ProjectId = Project.Id
        };
        var result = await ApiClient.SendAsync(request);

        if (!result.IsError)
        {
            await ProjectChanged.InvokeAsync(Project with
            { 
                LeadAnalyst = request.AnalystId is null ? null : new()
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName
                } 
            });
        }

        IsBusy = false;
    }
}
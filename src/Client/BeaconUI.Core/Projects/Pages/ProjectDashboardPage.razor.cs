using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Requests.Projects;
using BeaconUI.Core.Common.Http;
using BeaconUI.Core.Common.Modals;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Pages;

public partial class ProjectDashboardPage
{
    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    [CascadingParameter]
    private IModalService ModalService { get; set; } = null!;

    private GetProjectsRequest Request { get; set; } = new();
    private ErrorOr<LaboratoryMemberDto[]>? AnalystsOrError { get; set; }
    private ErrorOr<PagedList<ProjectDto>>? ProjectsOrError { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Request.PageSize = 10;
        Request.IncludedStatuses = [ProjectStatus.Active, ProjectStatus.Pending];
        await Task.WhenAll(LoadProjects(), LoadLeadAnalystOptions());
    }

    private async Task GoToPage(int page)
    {
        Request.PageNumber = page;
        await LoadProjects();
    }

    private async Task LoadProjects()
    {
        ProjectsOrError = await ApiClient.SendAsync(Request);
    }

    private async Task LoadLeadAnalystOptions()
    {
        var request = new GetAnalystsRequest { IncludeHistoricAnalysts = true };
        AnalystsOrError = await ApiClient.SendAsync(request);
    }

    private async Task CancelProject(ProjectDto project)
    {
        var parameters = new ModalParameters().Add(nameof(AreYouSureModal.Content), $"Really cancel project {project.ProjectCode}?");
        var result = await ModalService.Show<AreYouSureModal>("Confirm Action", parameters).Result;
        if (result.Confirmed)
        {
            await ApiClient.SendAsync(new CancelProjectRequest { ProjectId = project.Id });
            await LoadProjects();
        }
    }

    private async Task CompleteProject(ProjectDto project)
    {
        var parameters = new ModalParameters().Add(nameof(AreYouSureModal.Content), $"Really complete project {project.ProjectCode}?");
        var result = await ModalService.Show<AreYouSureModal>("Confirm Action", parameters).Result;
        if (result.Confirmed)
        {
            await ApiClient.SendAsync(new CompleteProjectRequest { ProjectId = project.Id });
            await LoadProjects();
        }
    }

    private async Task Toggle(ProjectStatus status)
    {
        if (Request.IncludedStatuses.Contains(status))
            Request.IncludedStatuses.Remove(status);
        else
            Request.IncludedStatuses.Add(status);

        await LoadProjects();
    }

    private async Task ToggleLeadAnalyst(LaboratoryMemberDto? analyst)
    {
        var id = analyst?.Id ?? Guid.Empty;

        if (Request.LeadAnalystIds.Contains(id))
            Request.LeadAnalystIds.Remove(id);
        else
            Request.LeadAnalystIds.Add(id);

        await LoadProjects();
    }
}
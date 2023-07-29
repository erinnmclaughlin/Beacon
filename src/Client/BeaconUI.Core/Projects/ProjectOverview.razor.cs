using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Events;
using BeaconUI.Core.Common.Http;
using ErrorOr;
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

    private ErrorOr<ProjectEventDto[]>? Events { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadEvents();
    }

    private async Task LoadEvents()
    {
        Events = await ApiClient.SendAsync(new GetProjectEventsRequest
        {
            ProjectId = Project.Id
        });
    }

    private IEnumerable<ProjectEventDto> GetOngoingEvents(ProjectEventDto[] events) => events.Where(e => e.ScheduledStart <= DateTime.Today && e.ScheduledEnd >= DateTime.Today);
    private IEnumerable<ProjectEventDto> GetUpcomingEvents(ProjectEventDto[] events) => events.Where(e => e.ScheduledStart > DateTime.Today);
    private IEnumerable<ProjectEventDto> GetPastEvents(ProjectEventDto[] events) => events.Where(e => e.ScheduledEnd < DateTime.Today);
}
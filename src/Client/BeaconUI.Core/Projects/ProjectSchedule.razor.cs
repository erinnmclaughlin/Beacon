using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Events;
using BeaconUI.Core.Common;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects;

public partial class ProjectSchedule
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }
    private ErrorOr<ProjectEventDto[]>? ErrorOrEvents { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadProjects();
    }

    private static IEnumerable<TimelineItem<ProjectEventDto>> GetTimelineEvents(ProjectEventDto[] events)
    {
        return events.Select(e => new TimelineItem<ProjectEventDto> { Timestamp = e.ScheduledStart.DateTime, Value = e });
    }

    private async Task LoadProjects()
    {
        ErrorOrEvents = await ApiClient.SendAsync(new GetProjectEventsRequest { ProjectId = ProjectId });
    }
}
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

    protected override async Task OnParametersSetAsync()
    {
        await LoadEvents();
    }

    private async Task LoadEvents()
    {
        Events = await ApiClient.SendAsync(new GetProjectEventsRequest
        {
            MinEnd = DateTime.UtcNow,
            MaxStart = DateTime.UtcNow.AddDays(7),
            ProjectId = Project.Id
        });
    }
}
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

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    private static DateOnly ThisMonth => new(Today.Year, Today.Month, 1);

    protected override async Task OnInitializedAsync()
    {
        await LoadEvents();
    }

    private async Task LoadEvents()
    {
        Events = await ApiClient.SendAsync(new GetProjectEventsRequest
        {
            MinDate = Today,
            MaxDate = ThisMonth.AddMonths(1),
            ProjectId = Project.Id
        });
    }
}
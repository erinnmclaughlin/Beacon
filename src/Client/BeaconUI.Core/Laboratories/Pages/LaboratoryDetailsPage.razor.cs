using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Requests.Projects;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Laboratories.Pages;

public partial class LaboratoryDetailsPage
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    private ErrorOr<GetProjectTypeFrequencyRequest.Series[]>? ProjectTypeFrequencies { get; set; }
    private ErrorOr<PagedList<LaboratoryEventDto>>? Events { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Task.WhenAll(LoadAnalytics(), LoadEvents());
    }

    private async Task LoadAnalytics()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        ProjectTypeFrequencies = await ApiClient.SendAsync(new GetProjectTypeFrequencyRequest
        {
            StartDate = today.AddYears(-1)
        });
    }

    private async Task LoadEvents()
    {
        Events = await ApiClient.SendAsync(new GetLaboratoryEventsRequest
        {
            MinEnd = DateTime.UtcNow,
            MaxStart = DateTime.UtcNow.AddDays(7)
        });
    }

}

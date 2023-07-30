using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Laboratories.Pages;

public partial class LaboratoryDetailsPage
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    private ErrorOr<LaboratoryEventDto[]>? Events { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadEvents();
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

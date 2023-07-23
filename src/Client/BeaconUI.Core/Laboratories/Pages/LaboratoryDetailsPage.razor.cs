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
    
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    private static DateOnly ThisMonth => new(Today.Year, Today.Month, 1);

    protected override async Task OnInitializedAsync()
    {
        await LoadEvents();
    }

    private async Task LoadEvents()
    {
        Events = await ApiClient.SendAsync(new GetLaboratoryEventsRequest
        {
            MinDate = Today,
            MaxDate = ThisMonth.AddMonths(1)
        });
    }
}

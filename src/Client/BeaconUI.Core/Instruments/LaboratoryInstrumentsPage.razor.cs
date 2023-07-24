using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Instruments;
public partial class LaboratoryInstrumentsPage
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    private ErrorOr<LaboratoryInstrumentDto[]>? ErrorOrInstruments { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadInstruments();
    }

    private async Task LoadInstruments()
    {
        ErrorOrInstruments = await ApiClient.SendAsync(new GetLaboratoryInstrumentsRequest());
    }
}

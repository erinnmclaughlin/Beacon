using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using BeaconUI.Core.Common.Http;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core;

public partial class Index
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    private LaboratoryDto[]? Laboratories { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadMyLaboratories();
    }

    private async Task LoadMyLaboratories()
    {
        var errorOrLaboratories = await ApiClient.SendAsync(new GetMyLaboratoriesRequest());
        Laboratories = errorOrLaboratories.IsError ? null : errorOrLaboratories.Value;
    }
}
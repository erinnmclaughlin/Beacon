using Beacon.Common.Laboratories;
using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Shared.Laboratories;

public sealed partial class MyLaboratories
{
    [Inject] private AuthClient AuthClient { get; set; } = null!;

    [Parameter] public RenderFragment<List<LaboratoryDto>>? ChildContent { get; set; }

    private List<LaboratoryDto> Memberships { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadMembershipsAsync();
    }

    private async Task LoadMembershipsAsync()
    {
        var currentUserOrError = await AuthClient.GetCurrentUserAsync();
        Memberships = currentUserOrError.IsError ? new() : currentUserOrError.Value.Laboratories.ToList();
    }
}

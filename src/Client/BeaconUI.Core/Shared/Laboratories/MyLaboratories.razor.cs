using Beacon.Common.Laboratories;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Shared.Laboratories;

public sealed partial class MyLaboratories
{
    [Parameter]
    public RenderFragment<List<LaboratoryDto>>? ChildContent { get; set; }

    private List<LaboratoryDto> Memberships { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadMembershipsAsync();
    }

    private async Task LoadMembershipsAsync()
    {
        var currentUserOrError = await ApiClient.GetCurrentUser();
        Memberships = currentUserOrError.IsError ? new() : currentUserOrError.Value.Laboratories.ToList();
    }
}

using Beacon.Common.Laboratories;
using BeaconUI.Core.Clients;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Shared.Laboratories;

public sealed partial class MyLaboratories : IDisposable
{
    [Inject] private LabClient LabClient { get; set; } = null!;

    [Parameter] public RenderFragment<List<LaboratoryMembershipDto>>? ChildContent { get; set; }

    private List<LaboratoryMembershipDto> Memberships { get; set; } = new();

    public void Dispose()
    {
        LabClient.OnCurrentUserMembershipsChanged -= HandleMembershipsChangedEvent;
    }

    protected override async Task OnInitializedAsync()
    {
        LabClient.OnCurrentUserMembershipsChanged += HandleMembershipsChangedEvent;
        await LoadMembershipsAsync();
    }

    private async void HandleMembershipsChangedEvent()
    {
        await LoadMembershipsAsync();
        StateHasChanged();
    }

    private async Task LoadMembershipsAsync()
    {
        var membershipsOrError = await LabClient.GetCurrentUserMembershipsAsync();
        Memberships = membershipsOrError.Match(m => m, _ => new());
    }
}

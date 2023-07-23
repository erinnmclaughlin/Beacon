using Beacon.Common.Models;
using Beacon.Common.Services;
using BeaconUI.Core.Memberships.Modals;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Memberships.Components;

public partial class InviteMemberButton
{
    [CascadingParameter]
    public IModalService ModalService { get; set; } = default!;

    [CascadingParameter]
    public ILabContext LabContext { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? AdditionalAttributes { get; set; }

    private bool IsDisabled()
    {
        return LabContext.CurrentLab.MembershipType < LaboratoryMembershipType.Manager;
    }

    private async Task ShowInviteMemberModal()
    {
        await ModalService.Show<InviteMemberModal>("Invite Lab Member").Result;
    }
}
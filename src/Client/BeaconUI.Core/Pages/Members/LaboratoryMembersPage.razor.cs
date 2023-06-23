using Beacon.Common.Laboratories;
using Beacon.Common.Memberships;
using BeaconUI.Core.Shared.Laboratories;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Members;

public partial class LaboratoryMembersPage
{
    [CascadingParameter]
    private LaboratoryDto CurrentLab { get; set; } = null!;

    [CascadingParameter] 
    private IModalService ModalService { get; set; } = null!;

    private ErrorOr<LaboratoryMemberDto[]>? ErrorOrMembers { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ErrorOrMembers = await ApiClient.GetLaboratoryMembers(CurrentLab.Id);
    }

    private bool CanManagePermissions(LaboratoryMemberDto memberToUpdate)
    {
        if (CurrentLab.MyMembershipType is LaboratoryMembershipType.Admin)
            return true;

        if (CurrentLab.MyMembershipType is not LaboratoryMembershipType.Manager)
            return false;

        return memberToUpdate.MembershipType is not LaboratoryMembershipType.Admin;
    }

    private async Task ShowInviteMemberModal()
    {
        await ModalService.Show<InviteMemberForm>("Invite Lab Member").Result;
    }

    private async Task ShowManagePermissionsModal(LaboratoryMemberDto memberToUpdate)
    {
        var modalParameters = new ModalParameters().Add(nameof(UpdateMembershipTypeForm.MemberToUpdate), memberToUpdate);
        await ModalService.Show<UpdateMembershipTypeForm>("Update Membership", modalParameters).Result;
    }
}
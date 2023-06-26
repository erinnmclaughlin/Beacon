using Beacon.Common.Models;
using Beacon.Common.Services;
using BeaconUI.Core.Shared.Laboratories;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Pages.Members;

public partial class LaboratoryMembersPage
{
    [Inject]
    private ICurrentUser CurrentUser { get; set; } = null!;

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
        Console.WriteLine($"Current user id: {CurrentUser.UserId}");
        Console.WriteLine($"Member id: {memberToUpdate.Id}");

        if (memberToUpdate.Id == CurrentUser.UserId)
            return false;

        if (CurrentLab.MyMembershipType is LaboratoryMembershipType.Admin)
            return true;

        if (CurrentLab.MyMembershipType is not LaboratoryMembershipType.Manager)
            return false;

        return memberToUpdate.MembershipType is not LaboratoryMembershipType.Admin;
    }

    private async Task ShowInviteMemberModal()
    {
        await ModalService.Show<InviteMemberModal>("Invite Lab Member").Result;
    }

    private async Task ShowManagePermissionsModal(LaboratoryMemberDto memberToUpdate)
    {
        var modalParameters = new ModalParameters()
            .Add(nameof(UpdateMembershipTypeModal.CurrentLab), CurrentLab)
            .Add(nameof(UpdateMembershipTypeModal.MemberToUpdate), memberToUpdate);

        await ModalService.Show<UpdateMembershipTypeModal>("Update Membership", modalParameters).Result;
    }
}
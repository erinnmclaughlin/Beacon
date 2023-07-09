using Beacon.Common.Models;
using Beacon.Common.Services;
using BeaconUI.Core.Memberships.Modals;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Memberships.Pages;

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
        await LoadMemberships();
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

    private async Task LoadMemberships()
    {
        ErrorOrMembers = await ApiClient.GetLaboratoryMembers();
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

        var result = await ModalService.Show<UpdateMembershipTypeModal>("Update Membership", modalParameters).Result;

        if (result.Confirmed)
            await LoadMemberships();
    }
}
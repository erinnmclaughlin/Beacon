using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Enums;
using BeaconUI.Core.Shared.Laboratories;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class LaboratoryMembersPage
{
    [CascadingParameter] public required Task<AuthenticationState> AuthStateTask { get; set; }
    [CascadingParameter] public required LaboratoryDto CurrentLaboratory { get; set; }
    [CascadingParameter] private IModalService ModalService { get; set; } = null!;

    private AuthenticationState AuthState { get; set; } = null!;
    
    private ErrorOr<LaboratoryMemberDto[]>? ErrorOrMembers { get; set; }
    
    protected async override Task OnParametersSetAsync()
    {
        AuthState = await AuthStateTask;
        ErrorOrMembers = await ApiClient.GetLaboratoryMembers();
    }

    private bool CanManagePermissions(LaboratoryMemberDto memberToUpdate)
    {
        if (AuthState.User.FindFirst(BeaconClaimTypes.UserId)?.Value == memberToUpdate.Id.ToString())
            return false;

        if (CurrentLaboratory.MyMembershipType is LaboratoryMembershipType.Admin)
            return true;
        
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
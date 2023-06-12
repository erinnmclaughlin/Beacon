using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Enums;
using BeaconUI.Core.Shared.Laboratories;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeaconUI.Core.Pages.Laboratories;

public partial class LaboratoryMembersPage
{
    [CascadingParameter] public required Task<AuthenticationState> AuthStateTask { get; set; }
    [CascadingParameter] public required LaboratoryDetailDto Details { get; set; }
    [CascadingParameter] private IModalService ModalService { get; set; } = null!;

    private AuthenticationState AuthState { get; set; } = null !;
    
    protected async override Task OnParametersSetAsync()
    {
        AuthState = await AuthStateTask;
    }

    private bool CanManagePermissions(LaboratoryMemberDto memberToUpdate)
    {
        if (AuthState.User.FindFirst(BeaconClaimTypes.UserId)?.Value == memberToUpdate.Id.ToString())
            return false;

        if (Details.CurrentUserMembershipType is LaboratoryMembershipType.Admin)
            return true;
        
        return memberToUpdate.MembershipType is not LaboratoryMembershipType.Admin;
    }

    private async Task ShowInviteMemberModal()
    {
        await ModalService.Show<InviteMemberForm>("Invite Lab Member").Result;
    }

    private async Task ShowManagePermissionsModal(LaboratoryMemberDto memberToUpdate)
    {
        var modalParameters = new ModalParameters().Add(nameof(UpdateMembershipTypeForm.MemberToUpdate), new LaboratoryMembershipDto { Laboratory = new LaboratoryDto { Id = Details.Id, Name = Details.Name }, Member = memberToUpdate.ToUserDto(), MembershipType = memberToUpdate.MembershipType });
        await ModalService.Show<UpdateMembershipTypeForm>("Update Membership", modalParameters).Result;
    }
}
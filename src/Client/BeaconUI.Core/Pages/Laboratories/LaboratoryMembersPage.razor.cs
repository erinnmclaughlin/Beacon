using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
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
    [CascadingParameter] private IModalService ModalService { get; set; } = null!;

    private AuthenticationState AuthState { get; set; } = null!;
    
    private ErrorOr<LaboratoryMemberDto[]>? ErrorOrMembers { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        ErrorOrMembers = await ApiClient.GetLaboratoryMembers();
    }

    protected async override Task OnParametersSetAsync()
    {
        AuthState = await AuthStateTask;
    }

    private bool CanManagePermissions(LaboratoryMemberDto memberToUpdate)
    {
        if (SessionInfoDto.FromClaimsPrincipal(AuthState.User) is not { CurrentLab: { } } session)
            return false;

        if (session.CurrentUser.Id == memberToUpdate.Id)
            return false;

        if (session.CurrentLab.MembershipType is LaboratoryMembershipType.Admin)
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
using Beacon.Common.Auth;
using Beacon.Common.Memberships;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Shared.Laboratories;

public partial class UpdateMembershipTypeForm
{
    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = null!;

    [Parameter]
    public required LaboratoryMemberDto MemberToUpdate { get; set; }

    private UpdateMembershipTypeRequest Model { get; set; } = new();

    protected override void OnParametersSet()
    {
        Model.MembershipType = MemberToUpdate.MembershipType;
    }

    private async Task Submit(BeaconForm form)
    {
        var result = await ApiClient.UpdateMembershipType(MemberToUpdate.Id, Model);

        if (result.IsError)
        {
            form.AddErrors(result.Errors);
            return;
        }

        await Modal.CloseAsync(ModalResult.Ok());
    }

    private bool IsDisabled(SessionInfoDto sessionInfo, LaboratoryMembershipType targetType)
    {
        var myMembership = sessionInfo.CurrentLab?.MembershipType;

        if (myMembership is LaboratoryMembershipType.Admin)
            return false;

        if (myMembership is not LaboratoryMembershipType.Manager)
            return true;

        return MemberToUpdate.MembershipType is LaboratoryMembershipType.Admin || targetType is LaboratoryMembershipType.Admin;
    }
}
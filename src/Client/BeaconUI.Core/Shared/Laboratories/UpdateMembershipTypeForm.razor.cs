using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Enums;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Shared.Laboratories;

public partial class UpdateMembershipTypeForm
{
    [Inject]
    private LabClient LabClient { get; set; } = null !;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = null !;

    [Parameter]
    public required LaboratoryMembershipDto MemberToUpdate { get; set; }
    private UpdateMembershipTypeRequest Model { get; set; } = new();

    protected override void OnParametersSet()
    {
        Model.MembershipType = MemberToUpdate.MembershipType;
    }

    private async Task Submit(BeaconForm form)
    {
        var result = await LabClient.UpdateMembershipType(MemberToUpdate.Laboratory, MemberToUpdate.Member, Model);
        if (result.IsError)
        {
            form.AddErrors(result.Errors);
            return;
        }

        await Modal.CloseAsync(ModalResult.Ok());
    }

    private bool IsDisabled(LaboratoryMembershipDto? myMembership, LaboratoryMembershipType targetType)
    {
        if (myMembership is null || myMembership.Member.Id == MemberToUpdate.Member.Id)
            return true;
        if (myMembership?.MembershipType is LaboratoryMembershipType.Admin)
            return false;
        if (myMembership?.MembershipType is not LaboratoryMembershipType.Manager)
            return true;
        return MemberToUpdate.MembershipType is LaboratoryMembershipType.Admin || targetType is LaboratoryMembershipType.Admin;
    }
}
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
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

    [CascadingParameter]
    private LaboratoryDetailDto Detail { get; set; } = null!;

    [Parameter]
    public required LaboratoryMembershipDto MemberToUpdate { get; set; }

    private UpdateMembershipTypeRequest Model { get; set; } = new();

    protected override void OnParametersSet()
    {
        Model.MembershipType = MemberToUpdate.MembershipType;
    }

    private async Task Submit(BeaconForm form)
    {
        var result = await ApiClient.UpdateMembershipType(MemberToUpdate.Member.Id, Model);

        if (result.IsError)
        {
            form.AddErrors(result.Errors);
            return;
        }

        await Modal.CloseAsync(ModalResult.Ok());
    }

    private bool IsDisabled(LaboratoryMembershipType targetType)
    {
        var myMembership = Detail.CurrentUserMembershipType;

        if (myMembership is LaboratoryMembershipType.Admin)
            return false;

        if (myMembership is not LaboratoryMembershipType.Manager)
            return true;

        return MemberToUpdate.MembershipType is LaboratoryMembershipType.Admin || targetType is LaboratoryMembershipType.Admin;
    }
}
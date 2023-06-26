using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Shared.Forms;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Shared.Laboratories;

public partial class UpdateMembershipTypeForm
{
    [CascadingParameter]
    private LaboratoryDto CurrentLab { get; set; } = null!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = null!;

    [Parameter]
    public required LaboratoryMemberDto MemberToUpdate { get; set; }

    private UpdateMembershipRequest? _model;
    private UpdateMembershipRequest Model => _model ??= new()
    {
        MemberId = MemberToUpdate.Id,
        MembershipType = MemberToUpdate.MembershipType
    };

    private async Task Submit(BeaconForm form)
    {
        var result = await ApiClient.UpdateMembershipType(Model);

        if (result.IsError)
        {
            form.AddErrors(result.Errors);
            return;
        }

        await Modal.CloseAsync(ModalResult.Ok());
    }

    private bool IsDisabled(LaboratoryMembershipType targetType)
    {
        if (CurrentLab.MyMembershipType is LaboratoryMembershipType.Admin)
            return false;

        if (CurrentLab.MyMembershipType is not LaboratoryMembershipType.Manager)
            return true;

        return MemberToUpdate.MembershipType is LaboratoryMembershipType.Admin || targetType is LaboratoryMembershipType.Admin;
    }
}
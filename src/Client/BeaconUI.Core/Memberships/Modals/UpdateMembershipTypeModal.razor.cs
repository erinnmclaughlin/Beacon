using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Services;
using BeaconUI.Core.Common.Forms;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Memberships.Modals;

public partial class UpdateMembershipTypeModal
{
    [Inject]
    private ICurrentUser CurrentUser { get; set; } = null!;

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
        return CurrentUser.MembershipType is null or < LaboratoryMembershipType.Manager
            || CurrentUser.MembershipType <= targetType;
    }
}
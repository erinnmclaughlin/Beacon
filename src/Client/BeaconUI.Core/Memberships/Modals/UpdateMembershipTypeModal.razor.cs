using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Services;
using BeaconUI.Core.Common.Forms;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Memberships.Modals;

public partial class UpdateMembershipTypeModal
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = default!;

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
        var result = await ApiClient.SendAsync(Model);
        if (result.IsError)
        {
            form.AddErrors(result.Errors);
            return;
        }

        await Modal.CloseAsync(ModalResult.Ok(MemberToUpdate with
        {
            MembershipType = Model.MembershipType
        }));
    }

    private bool IsDisabled(ILabContext context, LaboratoryMembershipType targetType)
    {
        return context.CurrentLab.MembershipType < targetType;
    }
}
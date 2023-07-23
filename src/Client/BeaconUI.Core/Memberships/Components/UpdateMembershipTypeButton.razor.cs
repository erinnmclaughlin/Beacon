using Beacon.Common.Models;
using Beacon.Common.Services;
using BeaconUI.Core.Memberships.Modals;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Memberships.Components;

public partial class UpdateMembershipTypeButton
{
    [CascadingParameter]
    public ILabContext LabContext { get; set; } = default!;

    [CascadingParameter]
    public IModalService ModalService { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? AdditionalAttributes { get; set; }

    [Parameter, EditorRequired]
    public required LaboratoryMemberDto Member { get; set; }

    [Parameter, EditorRequired]
    public required EventCallback<LaboratoryMemberDto> MemberChanged { get; set; }
    
    private bool IsDisabled => !CanManagePermissions();

    private async Task Click()
    {
        var modalParameters = new ModalParameters().Add(nameof(UpdateMembershipTypeModal.MemberToUpdate), Member);
        var result = await ModalService.Show<UpdateMembershipTypeModal>("Update Membership", modalParameters).Result;

        if (result.Confirmed && result.Data is LaboratoryMemberDto updatedMember)
            await MemberChanged.InvokeAsync(updatedMember);
    }

    public bool CanManagePermissions()
    {
        return
            Member.Id != LabContext.CurrentUser.Id && 
            LabContext.CurrentLab.MembershipType >= Member.MembershipType;
    }
}
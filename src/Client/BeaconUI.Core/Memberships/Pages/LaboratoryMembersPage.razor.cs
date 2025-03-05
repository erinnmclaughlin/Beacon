using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Services;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Memberships.Pages;

public partial class LaboratoryMembersPage
{
    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    [CascadingParameter]
    private ILabContext LabContext { get; set; } = null!;

    private ErrorOr<LaboratoryMemberDto[]>? Result { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        Result = await ApiClient.SendAsync(new GetMembershipsRequest());
    }

    private bool CanManagePermissions()
    {
        return LabContext.CurrentLab.MembershipType >= LaboratoryMembershipType.Manager;
    }

    private void HandleMemberChanged(LaboratoryMemberDto updatedMember)
    {
        if (Result?.IsError is null or true)
            return;

        var membersArray = Result.Value.Value;
        Result = membersArray.Select(m => m.Id == updatedMember.Id ? updatedMember : m).ToArray();
    }
}

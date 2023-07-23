using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Memberships.Components;

public partial class MembershipContext
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [Parameter]
    public RenderFragment<MembershipContext>? ChildContent { get; set; }

    private ErrorOr<LaboratoryMemberDto[]>? Result { get; set; }
    public LaboratoryMemberDto[] Members => Result?.Value ?? Array.Empty<LaboratoryMemberDto>();

    protected override async Task OnParametersSetAsync()
    {
        Result = await ApiClient.SendAsync(new GetMembershipsRequest());
    }

    public bool IsLoading() => Result is null;
    public bool HasErrors() => Result?.IsError is true;
}
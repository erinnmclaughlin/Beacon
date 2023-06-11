using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class LabClient : ApiClientBase
{
    public LabClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<ErrorOr<LaboratoryDetailDto>> GetLaboratoryDetails(CancellationToken ct = default)
    {
        return await GetAsync<LaboratoryDetailDto>($"api/lab", ct);
    }

    public Task<ErrorOr<Success>> SendInvite(InviteLabMemberRequest request, CancellationToken ct = default)
    {
        return PostAsync($"api/invitations", request, ct);
    }

    public async Task<ErrorOr<Success>> UpdateMembershipType(UserDto member, UpdateMembershipTypeRequest request, CancellationToken ct = default)
    {
        return await PutAsync($"api/memberships/{member.Id}/membershipType", request, ct);
    }
}

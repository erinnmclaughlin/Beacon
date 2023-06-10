using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class LabClient : ApiClientBase
{
    public Action? OnCurrentUserMembershipsChanged;
    public Action<LaboratoryMembershipDto>? OnMembershipChanged;

    public LabClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<ErrorOr<LaboratorySummaryDto>> CreateLaboratoryAsync(CreateLaboratoryRequest request, CancellationToken ct = default)
    {
        var result = await PostAsync<LaboratorySummaryDto>("api/laboratories", request, ct);

        if (!result.IsError)
            OnCurrentUserMembershipsChanged?.Invoke();

        return result;
    }

    public Task<ErrorOr<List<LaboratoryMembershipDto>>> GetCurrentUserMembershipsAsync(CancellationToken ct = default)
    {
        return GetAsync<List<LaboratoryMembershipDto>>("api/users/me/memberships", ct);
    }

    public async Task<ErrorOr<LaboratoryDetailDto>> GetLaboratoryDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await GetAsync<LaboratoryDetailDto>($"api/laboratories/{id}", ct);
    }

    public Task<ErrorOr<Success>> InviteMemberAsync(Guid labId, InviteLabMemberRequest request, CancellationToken ct = default)
    {
        return PostAsync($"api/laboratories/{labId}/invitations", request, ct);
    }

    public async Task<ErrorOr<Success>> UpdateMembershipType(LaboratoryDto lab, UserDto member, UpdateMembershipTypeRequest request, CancellationToken ct = default)
    {
        var result = await PutAsync($"api/laboratories/{lab.Id}/memberships/{member.Id}/membershipType", request, ct);

        if (!result.IsError)
            OnMembershipChanged?.Invoke(new LaboratoryMembershipDto
            {
                Laboratory = lab,
                Member = member,
                MembershipType = request.MembershipType
            });

        return result;
    }
}

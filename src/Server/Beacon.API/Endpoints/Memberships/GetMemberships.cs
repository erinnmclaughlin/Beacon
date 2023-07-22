using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Memberships;

internal sealed class GetMembershipsHandler : IBeaconRequestHandler<GetMembershipsRequest, LaboratoryMemberDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetMembershipsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryMemberDto[]>> Handle(GetMembershipsRequest request, CancellationToken ct)
    {
        return await _dbContext.Memberships
            .Select(m => new LaboratoryMemberDto
            {
                Id = m.Member.Id,
                DisplayName = m.Member.DisplayName,
                EmailAddress = m.Member.EmailAddress,
                MembershipType = m.MembershipType
            })
            .ToArrayAsync(ct);
    }
}

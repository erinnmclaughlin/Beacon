using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Memberships;

internal sealed class GetMembershipsHandler : IBeaconRequestHandler<GetMembershipsRequest, LaboratoryMemberDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetMembershipsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryMemberDto[]>> Handle(GetMembershipsRequest request, CancellationToken ct)
    {
        var filterBuilder = new FilterBuilder<Membership>();
        
        if (request.MinimumRole is { } minimumRole)
        {
            var validRoles = Enum.GetValues<LaboratoryMembershipType>().Where(m => m >= minimumRole).ToList();
            filterBuilder.Add(m => validRoles.Contains(m.MembershipType));
        }

        return await _dbContext.Memberships
            .Where(filterBuilder.Build())
            .Select(m => new LaboratoryMemberDto
            {
                Id = m.Member.Id,
                DisplayName = m.Member.DisplayName,
                EmailAddress = m.Member.EmailAddress,
                MembershipType = m.MembershipType
            })
            .OrderBy(m => m.DisplayName)
            .ThenBy(m => m.EmailAddress)
            .ToArrayAsync(ct);
    }
}

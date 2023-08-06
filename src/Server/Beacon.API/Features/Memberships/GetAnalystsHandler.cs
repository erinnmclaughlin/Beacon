using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Memberships;

internal sealed class GetAnalystsHandler : IBeaconRequestHandler<GetAnalystsRequest, LaboratoryMemberDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetAnalystsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryMemberDto[]>> Handle(GetAnalystsRequest request, CancellationToken ct)
    {
        var membershipTypes = Enum
            .GetValues<LaboratoryMembershipType>()
            .Where(m => m >= LaboratoryMembershipType.Analyst)
            .ToList();

        var analysts = await _dbContext.Memberships
            .Where(m => membershipTypes.Contains(m.MembershipType))
            .Select(m => new LaboratoryMemberDto
            {
                Id = m.Member.Id,
                DisplayName = m.Member.DisplayName,
                EmailAddress = m.Member.EmailAddress,
                MembershipType = m.MembershipType
            })
            .ToListAsync(ct);

        if (request.IncludeHistoricAnalysts)
        {
            var historicAnalysts = await _dbContext.Memberships
                .Where(m => m.Member.Projects.Any())
                .Select(m => new LaboratoryMemberDto
                {
                    Id = m.Member.Id,
                    DisplayName = m.Member.DisplayName,
                    EmailAddress = m.Member.EmailAddress,
                    MembershipType = m.MembershipType
                })
                .ToListAsync(ct);
            
            analysts
                .AddRange(historicAnalysts
                    .Where(a => analysts
                        .All(aa => aa.Id != a.Id)));
        }

        return analysts
            .OrderBy(a => a.DisplayName)
            .ThenBy(a => a.EmailAddress)
            .ToArray();
    }
}

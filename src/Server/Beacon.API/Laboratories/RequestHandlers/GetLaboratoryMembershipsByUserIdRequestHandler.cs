using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Laboratories.RequestHandlers;

internal class GetLaboratoryMembershipsByUserIdRequestHandler : IApiRequestHandler<GetLaboratoryMembershipsByUserIdRequest, List<LaboratoryMembershipDto>>
{
    private readonly BeaconDbContext _dbContext;

    public GetLaboratoryMembershipsByUserIdRequestHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<LaboratoryMembershipDto>>> Handle(GetLaboratoryMembershipsByUserIdRequest request, CancellationToken cancellationToken)
    {
        return await _dbContext.LaboratoryMemberships
            .Where(m => m.Member.Id == request.UserId)
            .Select(m => new LaboratoryMembershipDto
            {
                LaboratoryId = m.Laboratory.Id,
                LaboratoryName = m.Laboratory.Name,
                MemberId = m.Member.Id,
                MemberDisplayName = m.Member.DisplayName,
                MembershipType = m.MembershipType.ToString()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

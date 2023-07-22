using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

internal sealed class GetMyLaboratoriesHandler : IBeaconRequestHandler<GetMyLaboratoriesRequest, LaboratoryDto[]>
{
    private readonly ISessionContext _context;
    private readonly BeaconDbContext _dbContext;

    public GetMyLaboratoriesHandler(ISessionContext context, BeaconDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryDto[]>> Handle(GetMyLaboratoriesRequest request, CancellationToken ct)
    {
        var currentUserId = _context.UserId;

        return await _dbContext.Memberships
            .Where(m => m.MemberId == currentUserId)
            .Select(m => new LaboratoryDto
            {
                Id = m.Laboratory.Id,
                Name = m.Laboratory.Name,
                MemberCount = m.Laboratory.Memberships.Count
            })
            .IgnoreQueryFilters()
            .ToArrayAsync(ct);
    }
}

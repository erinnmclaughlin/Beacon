using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Services;

public sealed class LaboratoryContext : ILabContext
{
    private readonly BeaconDbContext _dbContext;

    public Guid LaboratoryId { get; }

    public LaboratoryContext(BeaconDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;

        var idValue = httpContextAccessor.HttpContext?.Request.Headers["X-LaboratoryId"];
        LaboratoryId = Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
    }

    public async Task<LaboratoryMembershipType?> GetMembershipTypeAsync(Guid userId, CancellationToken ct = default)
    {
        if (LaboratoryId == Guid.Empty)
            return null;

        var m = await _dbContext.Memberships
            .Where(x => x.LaboratoryId == LaboratoryId && x.MemberId == userId)
            .Select(x => new { x.MembershipType })
            .FirstOrDefaultAsync(ct);

        return m?.MembershipType;
    }
}

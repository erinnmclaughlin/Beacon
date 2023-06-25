using Beacon.API.Persistence;
using Beacon.App.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Services;

public sealed class LaboratoryContext
{
    private readonly BeaconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LaboratoryContext(BeaconDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid LaboratoryId
    {
        get
        {
            var idValue = _httpContextAccessor.HttpContext?.Request.Headers["X-LaboratoryId"];
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }

    public async Task<Membership?> GetMembershipAsync(Guid userId, CancellationToken ct)
    {
        var labId = LaboratoryId;

        return await _dbContext.Memberships
            .Where(m => m.LaboratoryId == labId && m.MemberId == userId)
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);
    }
}

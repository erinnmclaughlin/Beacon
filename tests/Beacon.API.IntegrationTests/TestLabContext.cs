using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests;

public sealed class TestLabContext : ILabContext
{
    private readonly BeaconDbContext _dbContext;

    public Guid LaboratoryId { get; } = TestData.Lab.Id;

    public TestLabContext(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LaboratoryMembershipType?> GetMembershipTypeAsync(Guid userId, CancellationToken ct = default)
    {
        var membership = await _dbContext.Memberships
            .Where(m => m.LaboratoryId == LaboratoryId && m.MemberId == userId)
            .FirstOrDefaultAsync(ct);

        return membership?.MembershipType;
    }
}

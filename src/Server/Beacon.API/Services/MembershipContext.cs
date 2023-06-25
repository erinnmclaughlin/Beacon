using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Services;

public sealed class MembershipContext : IMembershipContext
{
    private readonly ICurrentUser _currentUser;
    private readonly BeaconDbContext _dbContext;
    private readonly ILabContext _labContext;

    private bool IsInitialized { get; set; }
    private LaboratoryMembershipType? MembershipType { get; set; }

    public MembershipContext(ICurrentUser currentUser, BeaconDbContext dbContext, ILabContext labContext)
    {
        _currentUser = currentUser;
        _dbContext = dbContext;
        _labContext = labContext;
    }

    public async Task<LaboratoryMembershipType?> GetMembershipTypeAsync(CancellationToken ct = default)
    {
        if (!IsInitialized)
        {
            var userId = _currentUser.UserId;

            var membership = await _dbContext.Memberships
                .Where(m => m.MemberId == userId && m.LaboratoryId == _labContext.LaboratoryId)
                .Select(m => new { m.MembershipType })
                .SingleOrDefaultAsync(ct);

            MembershipType = membership?.MembershipType;
        }

        return MembershipType;


    }
}

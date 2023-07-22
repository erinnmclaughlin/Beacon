using Beacon.API.Persistence;
using Beacon.Common.Requests.Memberships;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Memberships;

internal sealed class UpdateMembershipHandler : IBeaconRequestHandler<UpdateMembershipRequest>
{
    private readonly BeaconDbContext _dbContext;

    public UpdateMembershipHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateMembershipRequest request, CancellationToken ct)
    {
        var member = await _dbContext.Memberships.SingleAsync(m => m.MemberId == request.MemberId, ct);

        member.MembershipType = request.MembershipType;
        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}

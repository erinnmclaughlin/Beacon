using Beacon.API.Extensions;
using Beacon.API.Persistence;
using Beacon.Common.Requests.Memberships;
using ErrorOr;

namespace Beacon.API.Features.Memberships;

internal sealed class UpdateMembershipHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<UpdateMembershipRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Success>> Handle(UpdateMembershipRequest request, CancellationToken ct)
    {
        var memberOrError = await _dbContext.Memberships.SingleOrErrorAsync(m => m.MemberId == request.MemberId, ct);

        if (memberOrError.IsError)
        {
            return Error.Validation(metadata: new Dictionary<string, object>
            {
                [nameof(request.MemberId)] = new[] { "Member not found." }
            });
        }
        
        memberOrError.Value.MembershipType = request.MembershipType;
        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}

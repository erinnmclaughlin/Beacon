using Beacon.API.Persistence;
using Beacon.Common.Requests.Memberships;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Memberships;

public sealed class UpdateMembership : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut<UpdateMembershipRequest>("memberships").WithTags(EndpointTags.Memberships);
    }

    internal sealed class Handler : IRequestHandler<UpdateMembershipRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateMembershipRequest request, CancellationToken ct)
        {
            var member = await _dbContext.Memberships.SingleAsync(m => m.MemberId == request.MemberId, ct);

            member.MembershipType = request.MembershipType;
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

using Beacon.API.Persistence;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Services;
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
        private readonly ILabContext _labContext;

        public Handler(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task Handle(UpdateMembershipRequest request, CancellationToken ct)
        {
            var member = await _dbContext.Memberships
                .SingleAsync(m => m.LaboratoryId == _labContext.LaboratoryId && m.MemberId == request.MemberId, ct);

            member.MembershipType = request.MembershipType;
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

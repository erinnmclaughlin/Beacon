using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Memberships;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Memberships;

public sealed class GetMemberships : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("memberships", new GetMembershipsRequest()).WithTags(EndpointTags.Memberships);
    }

    internal sealed class Handler : IRequestHandler<GetMembershipsRequest, LaboratoryMemberDto[]>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly LaboratoryContext _labContext;

        public Handler(BeaconDbContext dbContext, LaboratoryContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task<LaboratoryMemberDto[]> Handle(GetMembershipsRequest request, CancellationToken ct)
        {
            var labId = _labContext.LaboratoryId;

            return await _dbContext.Memberships
                .Where(m => m.LaboratoryId == labId)
                .Select(m => new LaboratoryMemberDto
                {
                    Id = m.Member.Id,
                    DisplayName = m.Member.DisplayName,
                    EmailAddress = m.Member.EmailAddress,
                    MembershipType = m.MembershipType
                })
                .ToArrayAsync(ct);
        }
    }
}

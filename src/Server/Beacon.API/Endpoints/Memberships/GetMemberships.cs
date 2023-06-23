using Beacon.API.Endpoints;
using Beacon.API.Persistence;
using Beacon.Common.Memberships;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Memberships;

public sealed class GetMemberships : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet("memberships", async ([AsParameters] GetMembershipsRequest request, IMediator m, CancellationToken ct) =>
        {
            var response = await m.Send(request, ct);
            return Results.Ok(response);
        });

        builder.WithTags(EndpointTags.Memberships);
    }

    internal sealed class Handler : IRequestHandler<GetMembershipsRequest, LaboratoryMemberDto[]>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LaboratoryMemberDto[]> Handle(GetMembershipsRequest request, CancellationToken ct)
        {
            return await _dbContext.Memberships
                .Where(m => m.LaboratoryId == request.LaboratoryId)
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

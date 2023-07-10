using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class GetMyLaboratories : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("laboratories", new GetMyLaboratoriesRequest()).WithTags(EndpointTags.Laboratories);
    }

    internal sealed class Handler : IRequestHandler<GetMyLaboratoriesRequest, LaboratoryDto[]>
    {
        private readonly ISessionContext _context;
        private readonly BeaconDbContext _dbContext;

        public Handler(ISessionContext context, BeaconDbContext dbContext)
        {
            _context = context;
            _dbContext = dbContext;
        }

        public async Task<LaboratoryDto[]> Handle(GetMyLaboratoriesRequest request, CancellationToken ct)
        {
            var currentUserId = _context.UserId;

            return await _dbContext.Memberships
                .Where(m => m.MemberId == currentUserId)
                .Select(m => new LaboratoryDto
                {
                    Id = m.Laboratory.Id,
                    Name = m.Laboratory.Name,
                    MemberCount = m.Laboratory.Memberships.Count
                })
                .ToArrayAsync(ct);
        }
    }
}

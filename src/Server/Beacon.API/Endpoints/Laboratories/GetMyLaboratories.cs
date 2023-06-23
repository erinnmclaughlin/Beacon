using Beacon.API.Persistence;
using Beacon.App.Services;
using Beacon.Common.Laboratories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class GetMyLaboratories : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("laboratories", new Request()).WithTags(EndpointTags.Laboratories);
    }

    public sealed record Request : IRequest<LaboratoryDto[]>;

    internal sealed class Handler : IRequestHandler<Request, LaboratoryDto[]>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task<LaboratoryDto[]> Handle(Request request, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;

            return await _dbContext.Memberships
                .Where(m => m.MemberId == currentUserId)
                .Select(m => new LaboratoryDto
                {
                    Id = m.Laboratory.Id,
                    Name = m.Laboratory.Name,
                    MyMembershipType = m.MembershipType
                })
                .ToArrayAsync(ct);
        }
    }
}

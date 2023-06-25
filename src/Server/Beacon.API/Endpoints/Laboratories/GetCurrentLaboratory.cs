using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Requests.Laboratories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class GetCurrentLaboratory : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("laboratories/current", new GetCurrentLaboratoryRequest()).WithTags(EndpointTags.Laboratories);
    }

    internal sealed class Handler : IRequestHandler<GetCurrentLaboratoryRequest, LaboratoryDto>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext, ILabContext labContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public Task<LaboratoryDto> Handle(GetCurrentLaboratoryRequest request, CancellationToken ct)
        {
            return _dbContext.Memberships
                .Where(x => x.LaboratoryId == _labContext.LaboratoryId && x.MemberId == _currentUser.UserId)
                .Select(x => new LaboratoryDto
                {
                    Id = x.Laboratory.Id,
                    Name = x.Laboratory.Name,
                    MyMembershipType = x.MembershipType
                })
                .SingleAsync(ct);
        }
    }
}

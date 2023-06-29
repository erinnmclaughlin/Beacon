using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
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

        public async Task<LaboratoryDto> Handle(GetCurrentLaboratoryRequest request, CancellationToken ct)
        {
            var m = await _labContext.GetMembershipTypeAsync(_currentUser.UserId, ct);

            return await _dbContext.Laboratories
                .Where(x => x.Id == _labContext.LaboratoryId)
                .Select(x => new LaboratoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    MyMembershipType = m,
                    MemberCount = x.Memberships.Count()
                })
                .SingleAsync(ct);
        }
    }
}

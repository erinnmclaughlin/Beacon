using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Laboratories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class GetLaboratoryById : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("laboratories/{laboratoryId:Guid}", async (Guid laboratoryId, IMediator m, CancellationToken ct) =>
        {
            var request = new GetLaboratoryByIdRequest { LaboratoryId = laboratoryId };
            return Results.Ok(await m.Send(request, ct));
        });
    }

    internal sealed class Handler : IRequestHandler<GetLaboratoryByIdRequest, LaboratoryDto>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public Task<LaboratoryDto> Handle(GetLaboratoryByIdRequest request, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;

            return _dbContext.Memberships
                .Where(x => x.LaboratoryId == request.LaboratoryId && x.MemberId == currentUserId)
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

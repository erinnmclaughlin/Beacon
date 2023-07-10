using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class SetCurrentLaboratory : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<SetCurrentLaboratoryRequest>("laboratories/current").WithTags(EndpointTags.Laboratories);
    }

    internal sealed class Handler : IRequestHandler<SetCurrentLaboratoryRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;
        private readonly ISignInManager _signInManager;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext, ISignInManager signInManager)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
            _signInManager = signInManager;
        }

        public async Task Handle(SetCurrentLaboratoryRequest request, CancellationToken ct)
        {
            var currentLab = request.LaboratoryId is null ? null : await _dbContext.Memberships
                .Where(m => m.LaboratoryId == request.LaboratoryId && m.MemberId == _currentUser.UserId)
                .Select(m => new CurrentLabDto
                {
                    Id = m.Laboratory.Id,
                    Name = m.Laboratory.Name,
                    MembershipType = m.MembershipType
                })
                .SingleAsync(ct);

            var sessionInfo = await _dbContext.Users
                .Where(u => u.Id == _currentUser.UserId)
                .Select(u => new SessionContext
                {
                    CurrentLab = currentLab,
                    CurrentUser = new()
                    {
                        Id = u.Id,
                        DisplayName = u.DisplayName
                    }
                })
                .AsNoTracking()
                .SingleAsync(ct);

            await _signInManager.SignInAsync(sessionInfo.ToClaimsPrincipal());
        }
    }
}

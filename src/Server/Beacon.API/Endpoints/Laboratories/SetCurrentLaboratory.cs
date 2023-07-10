using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Exceptions;
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
        private readonly ISessionContext _currentUser;
        private readonly BeaconDbContext _dbContext;
        private readonly ISignInManager _signInManager;

        public Handler(ISessionContext currentUser, BeaconDbContext dbContext, ISignInManager signInManager)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
            _signInManager = signInManager;
        }

        public async Task Handle(SetCurrentLaboratoryRequest request, CancellationToken ct)
        {
            var newContext = await _dbContext.Memberships
                .Where(m => m.LaboratoryId == request.LaboratoryId && m.MemberId == _currentUser.UserId)
                .Select(m => new SessionContext
                {
                    CurrentUser = new() { Id = m.Member.Id, DisplayName = m.Member.DisplayName },
                    CurrentLab = new() { Id = m.Laboratory.Id, Name = m.Laboratory.Name, MembershipType = m.MembershipType }
                })
                .SingleOrDefaultAsync(ct)
                ?? throw new UserNotAllowedException("The current user is not a member of the specified lab.");

            await _signInManager.SignInAsync(newContext.ToClaimsPrincipal());
        }
    }
}

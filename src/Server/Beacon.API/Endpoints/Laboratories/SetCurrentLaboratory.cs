using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var membershipInfo = await _dbContext.Memberships
                .Where(m => m.LaboratoryId == request.LaboratoryId && m.MemberId == _currentUser.UserId)
                .Select(m => new
                {
                    m.Laboratory.Id,
                    m.MembershipType
                })
                .SingleAsync(ct);

            var identity = new ClaimsIdentity("AuthCookie");
            identity.AddClaims(new[]
            {
                new Claim(BeaconClaimTypes.UserId, _currentUser.UserId.ToString()),
                new Claim(BeaconClaimTypes.LabId, membershipInfo.Id.ToString()),
                new Claim(BeaconClaimTypes.MembershipType, membershipInfo.MembershipType.ToString())
            });

            await _signInManager.SignInAsync(new ClaimsPrincipal(identity));
        }
    }
}

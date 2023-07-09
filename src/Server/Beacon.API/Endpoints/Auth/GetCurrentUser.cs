using Beacon.API.Persistence;
using Beacon.App.Services;
using Beacon.Common.Models;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Auth;

public sealed class GetCurrentUser : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("users/current", new GetCurrentUserRequest()).WithTags(EndpointTags.Authentication);
    }

    internal sealed class Handler : IRequestHandler<GetCurrentUserRequest, CurrentUserDto>
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

        public async Task<CurrentUserDto> Handle(GetCurrentUserRequest request, CancellationToken ct)
        {
            var membership = _labContext.LaboratoryId == Guid.Empty ? null : await _dbContext.Memberships
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.LaboratoryId == _labContext.LaboratoryId && m.MemberId == _currentUser.UserId, ct);

            var membershipType = membership?.MembershipType;

            return await _dbContext.Users
                .Where(u => u.Id == _currentUser.UserId)
                .Select(u => new CurrentUserDto(u.Id, u.DisplayName, membershipType))
                .SingleAsync(ct);
        }
    }
}

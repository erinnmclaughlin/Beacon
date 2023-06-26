using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Auth;

public sealed class GetCurrentUser : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("users/current", new GetCurrentUserRequest()).RequireAuthorization().WithTags(EndpointTags.Authentication);
    }

    internal sealed class Handler : IRequestHandler<GetCurrentUserRequest, CurrentUserDto>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task<CurrentUserDto> Handle(GetCurrentUserRequest request, CancellationToken ct)
        {
            return await _dbContext.Users
                .Where(u => u.Id == _currentUser.UserId)
                .Select(u => new CurrentUserDto(u.Id, u.DisplayName))
                .SingleAsync(ct);
        }
    }
}

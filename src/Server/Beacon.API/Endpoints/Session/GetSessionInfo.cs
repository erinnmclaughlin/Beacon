﻿using Beacon.API.Persistence;
using Beacon.App.Services;
using Beacon.Common.Auth;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Session;

public sealed class GetSessionInfo : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("session", new Request()).RequireAuthorization().WithTags(EndpointTags.Authentication);
    }

    internal sealed record Request : IRequest<SessionInfoDto>;

    internal sealed class Handler : IRequestHandler<Request, SessionInfoDto>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task<SessionInfoDto> Handle(Request request, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;

            var currentUser = await _dbContext.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => new SessionInfoDto.UserDto(u.Id, u.DisplayName))
                .SingleAsync(ct);

            return new SessionInfoDto(currentUser);
        }
    }
}

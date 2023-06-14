using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Auth;

public static class GetCurrentUser
{
    public sealed record Query : IRequest<AuthUserDto>;

    internal sealed class Handler : IRequestHandler<Query, AuthUserDto>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IQueryService _queryService;

        public Handler(ICurrentUser currentUser, IQueryService queryService)
        {
            _currentUser = currentUser;
            _queryService = queryService;
        }

        public async Task<AuthUserDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            return await _queryService
                .QueryFor<User>()
                .Where(u => u.Id == currentUserId)
                .Select(u => new AuthUserDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress
                })
                .FirstAsync(cancellationToken);
        }
    }
}

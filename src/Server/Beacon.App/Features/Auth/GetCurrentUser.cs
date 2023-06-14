using Beacon.App.Entities;
using Beacon.App.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Auth;

public static class GetCurrentUser
{
    public sealed record Query : IRequest<Response>;

    public sealed record Response
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    public sealed record Laboratory(Guid Id, string Name);

    internal sealed class Handler : IRequestHandler<Query, Response>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IQueryService _queryService;

        public Handler(ICurrentUser currentUser, IQueryService queryService)
        {
            _currentUser = currentUser;
            _queryService = queryService;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            return await _queryService
                .QueryFor<User>()
                .Where(u => u.Id == currentUserId)
                .Select(u => new Response
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress
                })
                .FirstAsync(cancellationToken);
        }
    }
}

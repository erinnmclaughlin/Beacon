using Beacon.App.Entities;
using Beacon.App.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Users;

public static class GetUserById
{
    public sealed record Query(Guid UserId) : IRequest<Response>;
    public sealed record Response(UserDto? User);

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    public sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly IQueryService _queryService;

        public QueryHandler(IQueryService queryService)
        {
            _queryService = queryService;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _queryService
                .QueryFor<User>()
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    EmailAddress = u.EmailAddress
                })
                .FirstOrDefaultAsync(cancellationToken);

            return new Response(user);
        }
    }
}

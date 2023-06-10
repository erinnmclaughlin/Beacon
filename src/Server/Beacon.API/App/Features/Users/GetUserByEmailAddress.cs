using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Users;

public static class GetUserByEmailAddress
{
    public sealed record Query(string EmailAddress) : IRequest<Response>;

    public sealed record Response(UserDto? User);

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    internal sealed class QueryHandler : IRequestHandler<Query, Response>
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
                .Where(u => u.EmailAddress == request.EmailAddress)
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

using Beacon.App.Entities;
using Beacon.App.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Users;

public static class GetUserByCredentials
{
    public sealed record Query(string EmailAddress, string PlainTextPassword) : IRequest<Response>;
    public sealed record Response(UserDto? User);

    public sealed record UserDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
    }

    public sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IQueryService _queryService;

        public QueryHandler(IPasswordHasher passwordHasher, IQueryService queryService)
        {
            _passwordHasher = passwordHasher;
            _queryService = queryService;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _queryService
                .QueryFor<User>()
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.PlainTextPassword, user.HashedPassword, user.HashedPasswordSalt))
                return new Response(null);

            return new Response(new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                EmailAddress = user.EmailAddress
            });
        }
    }
}

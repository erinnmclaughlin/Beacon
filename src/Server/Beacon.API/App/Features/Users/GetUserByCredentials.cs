using Beacon.API.App.Services;
using Beacon.API.App.Services.Security;
using Beacon.API.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Users;

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
        private readonly IUnitOfWork _unitOfWork;

        public QueryHandler(IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.GetRepository<User>()
                .AsQueryable()
                .Where(u => u.EmailAddress == request.EmailAddress)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

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

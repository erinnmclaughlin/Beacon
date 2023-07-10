using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Auth;

public sealed class Register : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<RegisterRequest>("auth/register").WithTags(EndpointTags.Authentication);
    }

    internal sealed class Handler : IRequestHandler<RegisterRequest>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public Handler(BeaconDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(RegisterRequest request, CancellationToken ct)
        {
            await EnsureEmailIsUnique(request.EmailAddress, ct);

            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = request.DisplayName,
                EmailAddress = request.EmailAddress,
                HashedPassword = _passwordHasher.Hash(request.Password, out var salt),
                HashedPasswordSalt = salt
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task EnsureEmailIsUnique(string email, CancellationToken ct)
        {
            if (await _dbContext.Users.AnyAsync(u => u.EmailAddress == email, ct))
                throw new BeaconValidationException(nameof(RegisterRequest.EmailAddress), "An account with the specified email address already exists.");
        }
    }
}

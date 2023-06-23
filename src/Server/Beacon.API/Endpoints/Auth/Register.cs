using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Auth;
using FluentValidation;
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

    public class EmailAddressValidator : AbstractValidator<RegisterRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public EmailAddressValidator(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(r => r.EmailAddress)
                .MustAsync(BeAUniqueEmailAddress);
        }

        public async Task<bool> BeAUniqueEmailAddress(string email, CancellationToken ct)
        {
            var emailExists = await _dbContext.Users.AnyAsync(u => u.EmailAddress == email, ct);
            return !emailExists;
        }
    }

    public sealed class Handler : IRequestHandler<RegisterRequest>
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
    }
}

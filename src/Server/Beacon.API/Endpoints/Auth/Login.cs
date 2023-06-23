using Beacon.API.Persistence;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Auth;

public sealed class Login : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<LoginRequest>("auth/login").WithTags(EndpointTags.Authentication);
    }

    internal sealed class Handler : IRequestHandler<LoginRequest>
    {
        private readonly ISessionManager _currentSession;
        private readonly BeaconDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public Handler(ISessionManager currentSession, BeaconDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _currentSession = currentSession;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(LoginRequest request, CancellationToken ct)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress, ct);

            if (user is null || !_passwordHasher.Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt))
            {
                throw new BeaconValidationException(nameof(LoginRequest.EmailAddress), "Email address or password is incorrect.");
            }

            await _currentSession.SignInAsync(user.Id);
        }
    }
}

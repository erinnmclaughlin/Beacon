using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Auth;

public static class Login
{
    public sealed record Command(string EmailAddress, string PlainTextPassword) : IRequest;

    internal sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly ISessionManager _currentSession;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IQueryService _queryService;

        public CommandHandler(ISessionManager currentSession, IPasswordHasher passwordHasher, IQueryService queryService)
        {
            _currentSession = currentSession;
            _passwordHasher = passwordHasher;
            _queryService = queryService;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _queryService
                .QueryFor<User>()
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.PlainTextPassword, user.HashedPassword, user.HashedPasswordSalt))
            {
                throw new BeaconValidationException(nameof(Command.EmailAddress), "Email address or password is incorrect.");
            }

            await _currentSession.SignInAsync(user.Id);
        }
    }
}

using Beacon.App.Entities;
using Beacon.App.Services;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Auth;

public static class Login
{
    public sealed record Command(string EmailAddress, string PlainTextPassword) : IRequest;

    internal sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISignInManager _signInManager;
        private readonly IQueryService _queryService;

        public CommandHandler(IPasswordHasher passwordHasher, ISignInManager signInManager, IQueryService queryService)
        {
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
            _queryService = queryService;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _queryService
                .QueryFor<User>()
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.PlainTextPassword, user.HashedPassword, user.HashedPasswordSalt))
            {
                var failure = new ValidationFailure(nameof(Command.EmailAddress), "Email address or password is incorrect.");
                throw new ValidationException(new[] { failure });
            }

            await _signInManager.SignInAsync(user.Id);
        }
    }
}

using Beacon.App.Entities;
using Beacon.App.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Auth;

public static class Register
{
    public sealed record Command : IRequest
    {
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
        public required string PlainTextPassword { get; init; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(r => r.EmailAddress)
                .EmailAddress().WithMessage("Invalid email address.")
                .MustAsync(BeAUniqueEmailAddress).WithMessage("Email address is already in use.");

            RuleFor(r => r.DisplayName)
                .NotEmpty().WithMessage("Display name is required.");

            RuleFor(r => r.PlainTextPassword)
                .NotEmpty().WithMessage("Password is required.");
        }

        private async Task<bool> BeAUniqueEmailAddress(string emailAddress, CancellationToken ct)
        {
            var emailExists = await _unitOfWork.QueryFor<User>().AnyAsync(u => u.EmailAddress == emailAddress, ct);
            return !emailExists;
        }
    }

    public sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly ISessionManager _currentSession;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(ISessionManager currentSession, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _currentSession = currentSession;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = request.DisplayName,
                EmailAddress = request.EmailAddress,
                HashedPassword = _passwordHasher.Hash(request.PlainTextPassword, out var salt),
                HashedPasswordSalt = salt
            };

            _unitOfWork.GetRepository<User>().Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _currentSession.SignInAsync(user.Id);
        }
    }
}

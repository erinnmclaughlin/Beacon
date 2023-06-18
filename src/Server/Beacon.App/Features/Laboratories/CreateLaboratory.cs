using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Memberships;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class CreateLaboratory
{
    public sealed record Command(string Name) : IRequest;

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Name).IsValidLaboratoryName();
        }
    }

    internal sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly ISessionManager _currentSession;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(ISessionManager currentSession, IUnitOfWork unitOfWork)
        {
            _currentSession = currentSession;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            var currentUser = await GetCurrentUserAsync(ct);

            var lab = Laboratory.CreateNew(request.Name, currentUser);

            _unitOfWork.GetRepository<Laboratory>().Add(lab);
            await _unitOfWork.SaveChangesAsync(ct);

            await _currentSession.SetCurrentLabAsync(lab.Id, LaboratoryMembershipType.Admin);
        }

        private async Task<User> GetCurrentUserAsync(CancellationToken ct)
        {
            var currentUserId = _currentSession.UserId;

            return await _unitOfWork
                .QueryFor<User>(enableChangeTracking: true)
                .FirstAsync(u => u.Id == currentUserId, ct);
        }
    }
}

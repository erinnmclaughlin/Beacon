using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Laboratories;
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
        private readonly ISessionManager _sessionManager;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(ISessionManager sessionManager, IUnitOfWork unitOfWork)
        {
            _sessionManager = sessionManager;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            var currentUser = await GetCurrentUserAsync(ct);

            var lab = Laboratory.CreateNew(request.Name, currentUser);

            _unitOfWork.GetRepository<Laboratory>().Add(lab);
            await _unitOfWork.SaveChangesAsync(ct);

            await _sessionManager.SetCurrentLabAsync(lab.Id, LaboratoryMembershipType.Admin);
        }

        private async Task<User> GetCurrentUserAsync(CancellationToken ct)
        {
            var currentUserId = _sessionManager.UserId;

            return await _unitOfWork
                .QueryFor<User>(enableChangeTracking: true)
                .FirstAsync(u => u.Id == currentUserId, ct);
        }
    }
}

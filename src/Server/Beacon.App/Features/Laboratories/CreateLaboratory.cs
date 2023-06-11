using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;

namespace Beacon.App.Features.Laboratories;

public static class CreateLaboratory
{
    public sealed record Command : IRequest
    {
        public Guid LaboratoryId { get; private init; } = Guid.NewGuid();
        public required string LaboratoryName { get; set; }
    }

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.LaboratoryName).IsValidLaboratoryName();
        }
    }

    internal sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var currentUser = await _currentUser.GetCurrentUserAsync(cancellationToken);

            var lab = Laboratory.CreateNew(request.LaboratoryId, request.LaboratoryName, currentUser);

            _unitOfWork.GetRepository<Laboratory>().Add(lab);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

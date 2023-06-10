using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Beacon.API.Persistence;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Laboratories;

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
        private readonly BeaconDbContext _dbContext;

        public CommandHandler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var currentUser = await _dbContext.Users
                .FirstAsync(u => u.Id == _currentUser.UserId, cancellationToken);

            var lab = Laboratory.CreateNew(request.LaboratoryId, request.LaboratoryName, currentUser);

            _dbContext.Laboratories.Add(lab);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

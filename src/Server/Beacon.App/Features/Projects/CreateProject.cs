using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.App.ValueObjects;
using Beacon.Common.Memberships;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;

namespace Beacon.App.Features.Projects;

public static class CreateProject
{
    public sealed record Command(ProjectCode ProjectCode, string CustomerName) : IRequest;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CustomerName).IsValidCustomerName();
        }
    }

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ISessionManager _currentSession;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ISessionManager currentSession, IUnitOfWork unitOfWork)
        {
            _currentSession = currentSession;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            if (_currentSession.MembershipType is LaboratoryMembershipType.Member)
                throw new UserNotAllowedException("Only laboratory analysts or higher are allowed to create new projects.");

            _unitOfWork.GetRepository<Project>().Add(new Project
            {
                Id = Guid.NewGuid(),
                ProjectCode = request.ProjectCode,
                CustomerName = request.CustomerName,
                CreatedById = _currentSession.UserId,
                LaboratoryId = _currentSession.LabId
            });

            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}

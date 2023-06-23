using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.App.ValueObjects;
using Beacon.Common.Memberships;
using Beacon.Common.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Projects;

public static class CancelProject
{
    public sealed record Command(ProjectCode ProjectCode) : IRequest;

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ICurrentLab _currentLab;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ICurrentLab currentLab, IUnitOfWork unitOfWork)
        {
            _currentLab = currentLab;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            if (_currentLab.MembershipType is LaboratoryMembershipType.Member)
                throw new UserNotAllowedException();

            var labId = _currentLab.LabId;

            var project = await _unitOfWork
                .QueryFor<Project>(enableChangeTracking: true)
                .Where(p =>
                    p.LaboratoryId == labId &&
                    p.ProjectCode.CustomerCode == request.ProjectCode.CustomerCode &&
                    p.ProjectCode.Suffix == request.ProjectCode.Suffix)
                .SingleAsync(ct);

            if (project.ProjectStatus is ProjectStatus.Canceled)
                return;

            if (project.ProjectStatus is ProjectStatus.Completed)
                throw new BeaconValidationException(nameof(Project.ProjectStatus), "Projects that have been completed cannot be marked as canceled.");

            project.ProjectStatus = ProjectStatus.Canceled;
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}

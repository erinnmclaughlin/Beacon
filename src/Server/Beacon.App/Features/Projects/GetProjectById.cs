using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Projects;

public static class GetProjectById
{
    public sealed record Query(Guid Id) : IRequest<ProjectDto?>;

    internal sealed class Handler : IRequestHandler<Query, ProjectDto?>
    {
        private readonly ICurrentLab _currentLab;
        private readonly IQueryService _queryService;

        public Handler(ICurrentLab currentLab, IQueryService queryService)
        {
            _currentLab = currentLab;
            _queryService = queryService;
        }

        public async Task<ProjectDto?> Handle(Query request, CancellationToken ct)
        {
            var labId = _currentLab.LabId;

            var project = await _queryService
                .QueryFor<Project>()
                .Where(p => p.LaboratoryId == labId && p.Id == request.Id)
                .FirstOrDefaultAsync(ct);

            return project == null ? null : new ProjectDto
            {
                Id = project.Id,
                CustomerName = project.CustomerName,
                ProjectCode = project.ProjectCode.ToString(),
                ProjectStatus = project.ProjectStatus
            };
        }
    }
}

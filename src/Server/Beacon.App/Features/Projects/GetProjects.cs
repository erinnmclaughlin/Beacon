using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Projects;

public static class GetProjects
{
    public sealed record Query : IRequest<ProjectDto[]>;

    internal sealed class Handler : IRequestHandler<Query, ProjectDto[]>
    {
        private readonly ICurrentLab _currentLab;
        private readonly IQueryService _queryService;

        public Handler(ICurrentLab currentLab, IQueryService queryService)
        {
            _currentLab = currentLab;
            _queryService = queryService;
        }

        public async Task<ProjectDto[]> Handle(Query request, CancellationToken ct)
        {
            var labId = _currentLab.LabId;

            var projects = await _queryService
                .QueryFor<Project>()
                .Where(p => p.LaboratoryId == labId)
                .ToListAsync(ct);

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                CustomerName = p.CustomerName,
                ProjectCode = p.ProjectCode.ToString(),
                ProjectStatus = p.ProjectStatus
            }).ToArray();
        }
    }
}

using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.App.ValueObjects;
using Beacon.Common.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Projects;

public static class GetProjectByCode
{
    public sealed record Query(ProjectCode ProjectCode) : IRequest<ProjectDto?>;

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
                .FirstOrDefaultAsync(p => 
                    p.LaboratoryId == labId &&
                    p.ProjectCode.CustomerCode == request.ProjectCode.CustomerCode && 
                    p.ProjectCode.Suffix == request.ProjectCode.Suffix,
                ct);

            return project is null ? null : new ProjectDto
            {
                Id = project.Id,
                CustomerName = project.CustomerName,
                ProjectCode = project.ProjectCode.ToString()
            };
        }
    }
}

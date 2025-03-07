﻿using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class GetProjectByIdHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<GetProjectByIdRequest, ProjectDto>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<ProjectDto>> Handle(GetProjectByIdRequest request, CancellationToken ct)
    {
        var project = await _dbContext.Projects
                .Where(x => x.Id == request.ProjectId)
                .Select(x => new ProjectDto
                {
                    Id = x.Id,
                    CustomerName = x.CustomerName,
                    ProjectStatus = x.ProjectStatus,
                    ProjectCode = x.ProjectCode.ToString(),
                    Applications = x.TaggedApplications.Select(a => a.Application.Name).ToArray(),
                    LeadAnalyst = x.LeadAnalyst == null ? null : new ProjectDto.LeadAnalystDto
                    {
                        Id = x.LeadAnalyst.Id,
                        DisplayName = x.LeadAnalyst.DisplayName
                    }
                })
                .AsNoTracking()
                .SingleOrDefaultAsync(ct);

        return project is null ? Error.NotFound("Project not found.") : project;
    }
}

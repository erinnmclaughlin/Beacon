using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects;

internal sealed class GetProjectByProjectCodeHandler : IBeaconRequestHandler<GetProjectByProjectCodeRequest, ProjectDto>
{
    private readonly BeaconDbContext _dbContext;

    public GetProjectByProjectCodeHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProjectDto>> Handle(GetProjectByProjectCodeRequest request, CancellationToken ct)
    {
        var project = await _dbContext.Projects
                .Include(x => x.LeadAnalyst)
                .AsNoTracking()
                .SingleOrDefaultAsync(x =>
                    x.ProjectCode.CustomerCode == request.ProjectCode.CustomerCode &&
                    x.ProjectCode.Suffix == request.ProjectCode.Suffix, ct);

        return project is null ? Error.NotFound("Project not found.") : new ProjectDto
        {
            Id = project.Id,
            CustomerName = project.CustomerName,
            ProjectStatus = project.ProjectStatus,
            ProjectCode = project.ProjectCode.ToString(),
            LeadAnalyst = project.LeadAnalyst == null ? null : new ProjectDto.LeadAnalystDto
            {
                Id = project.LeadAnalyst.Id,
                DisplayName = project.LeadAnalyst.DisplayName
            }
        };
    }
}

using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class GetProjectByIdHandler : IBeaconRequestHandler<GetProjectByIdRequest, ProjectDto>
{
    private readonly BeaconDbContext _dbContext;

    public GetProjectByIdHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProjectDto>> Handle(GetProjectByIdRequest request, CancellationToken ct)
    {
        var project = await _dbContext.Projects
                .Include(x => x.LeadAnalyst)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.ProjectId, ct);

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

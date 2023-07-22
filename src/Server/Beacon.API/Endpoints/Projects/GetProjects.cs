using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects;

internal sealed class GetProjectsHandler : IBeaconRequestHandler<GetProjectsRequest, ProjectDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetProjectsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProjectDto[]>> Handle(GetProjectsRequest request, CancellationToken ct)
    {
        var projects = await _dbContext.Projects
            .Include(x => x.LeadAnalyst)
            .AsNoTracking()
            .ToArrayAsync(ct);

        return projects.Select(x => new ProjectDto
        {
            Id = x.Id,
            CustomerName = x.CustomerName,
            ProjectStatus = x.ProjectStatus,
            ProjectCode = x.ProjectCode.ToString(),
            LeadAnalyst = x.LeadAnalyst == null ? null : new ProjectDto.LeadAnalystDto
            {
                Id = x.LeadAnalyst.Id,
                DisplayName = x.LeadAnalyst.DisplayName
            }
        }).ToArray();
    }
}

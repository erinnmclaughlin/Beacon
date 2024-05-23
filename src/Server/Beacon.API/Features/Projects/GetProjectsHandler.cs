using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Beacon.API.Features.Projects;

internal sealed class GetProjectsHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<GetProjectsRequest, PagedList<ProjectDto>>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<PagedList<ProjectDto>>> Handle(GetProjectsRequest request, CancellationToken ct)
    {
        return await _dbContext.Projects
            .Where(GetFilter(request))
            .OrderBy(x => x.ProjectCode.CustomerCode)
            .ThenBy(x => x.ProjectCode.Date)
            .ThenBy(x => x.ProjectCode.Suffix)
            .ThenBy(x => x.Id)
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
            .ToPagedListAsync(request, ct);
    }

    private static Expression<Func<Project, bool>> GetFilter(GetProjectsRequest request)
    {
        var builder = new FilterBuilder<Project>();

        if (request.IncludedStatuses is { Count: > 0 } includedStatuses)
            builder.Add(project => includedStatuses.Contains(project.ProjectStatus));

        if (request.LeadAnalystIds.Select(id => id == Guid.Empty ? (Guid?)null: id).ToList() is { Count: > 0 } analystIds)
        {
            builder.Add(project => analystIds.Contains(project.LeadAnalystId));
        }

        return builder.Build();
    }
}

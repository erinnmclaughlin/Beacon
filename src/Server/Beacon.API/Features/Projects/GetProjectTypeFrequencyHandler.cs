using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;
internal sealed class GetProjectTypeFrequencyHandler : IBeaconRequestHandler<GetProjectTypeFrequencyRequest, GetProjectTypeFrequencyRequest.Series[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetProjectTypeFrequencyHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<GetProjectTypeFrequencyRequest.Series[]>> Handle(GetProjectTypeFrequencyRequest request, CancellationToken cancellationToken)
    {
        var query = _dbContext.ProjectApplicationTags.AsQueryable();

        if (request.StartDate?.ToDateTime(TimeOnly.MinValue) is { } start)
        {
            query = query.Where(p => p.Project.CreatedOn >= start);
        }

        if (request.EndDate?.ToDateTime(TimeOnly.MinValue) is { } end)
        {
            query = query.Where(p => p.Project.CreatedOn <= end);
        }

        var projects = await query
            .GroupBy(p => new { p.Application.Name, p.Project.CreatedOn })
            .Select(group => new
            {
                Date = group.Key.CreatedOn,
                ProjectType = group.Key.Name,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        return projects
            .GroupBy(p => p.ProjectType)
            .Select(group => new GetProjectTypeFrequencyRequest.Series
            {
                ProjectType = group.Key,
                ProjectCountByDate = projects
                    .Where(p => p.ProjectType == group.Key)
                    .GroupBy(p => DateOnly.FromDateTime(p.Date))
                    .ToDictionary(
                        p => p.Key, 
                        p => projects
                            .Where(pp => pp.ProjectType == group.Key && p.Key == DateOnly.FromDateTime(pp.Date))
                            .Sum(p => p.Count))
            })
            .ToArray();
    }
}

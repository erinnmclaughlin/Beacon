using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class GetProjectTypeFrequencyHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<GetProjectTypeFrequencyRequest, GetProjectTypeFrequencyRequest.Series[]>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<GetProjectTypeFrequencyRequest.Series[]>> Handle(GetProjectTypeFrequencyRequest request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate.ToDateTimeOffset();
        
        var projects = await _dbContext.ProjectApplicationTags
            .Where(p => p.Project.CreatedOn >= startDate)
            .GroupBy(p => new { p.Application.Name, p.Project.CreatedOn })
            .Select(group => new
            {
                Date = new DateOnly(group.Key.CreatedOn.Year, group.Key.CreatedOn.Month, 1),
                ProjectType = group.Key.Name,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        return projects
            .GroupBy(p => p.ProjectType)
            .Select(group => new GetProjectTypeFrequencyRequest.Series
            {
                ProjectType = group.Key,
                ProjectCountByDate = GetMonths(request.StartDate)
                    .ToDictionary(m => m, m => projects
                        .Where(p => p.ProjectType == group.Key && p.Date == m)
                        .Sum(p => p.Count))
            })
            .ToArray();
    }

    private static IEnumerable<DateOnly> GetMonths(DateOnly startDate)
    {
        var start = new DateOnly(startDate.Year, startDate.Month, 1);

        for (var i = 0; i < 12; i++)
            yield return start.AddMonths(i);
    }
}

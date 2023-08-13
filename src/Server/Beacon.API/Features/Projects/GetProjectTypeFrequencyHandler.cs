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
        var projects = await _dbContext.ProjectApplicationTags
            .Where(p => p.Project.CreatedOn >= request.StartDate)
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
                ProjectCountByDate = GetMonths()
                    .ToDictionary(m => m, m => projects
                        .Where(p => p.ProjectType == group.Key && p.Date == m)
                        .Sum(p => p.Count))
            })
            .ToArray();
    }

    private static IEnumerable<DateOnly> GetMonths()
    {
        var today = DateTime.Today;
        var start = new DateOnly(today.Year - 1, today.Month, 1);

        for (var i = 0; i < 12; i++)
            yield return start.AddMonths(i);
    }
}

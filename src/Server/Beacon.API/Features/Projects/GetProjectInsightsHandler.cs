using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal class GetProjectInsightsHandler : IBeaconRequestHandler<GetProjectInsightsRequest, ProjectInsightDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetProjectInsightsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProjectInsightDto[]>> Handle(GetProjectInsightsRequest request, CancellationToken ct)
    {
        var thisYear = DateTime.Today.AddYears(-1);
        var lastYear = thisYear.AddYears(-1);

        var stats = await _dbContext.ProjectApplications
            .Select(a => new ProjectApplicationPopularityStatistic
            {
                ApplicationType = a.Name,
                NumberOfProjectsCreatedThisYear = a.TaggedProjects.Count(p => p.Project.CreatedOn >= thisYear),
                NumberOfProjectsCreatedLastYear = a.TaggedProjects.Count(p => p.Project.CreatedOn >= lastYear && p.Project.CreatedOn < thisYear)
            })
            .Where(x => x.NumberOfProjectsCreatedThisYear != 0 && x.NumberOfProjectsCreatedLastYear != 0)
            .AsNoTracking()
            .ToListAsync(ct);

        var results = GetGrowthInsights(stats).ToArray();

        return results;
    }

    private static IEnumerable<ProjectInsightDto> GetGrowthInsights(List<ProjectApplicationPopularityStatistic> stats)
    {
        var growthSummary = new DataSummary(stats.Select(x => x.PercentGrowth * 100.0).ToList());

        foreach (var stat in stats)
        {
            var percentGrowth = 100.0 * stat.PercentGrowth;
            var percentGrowthWeight = Math.Abs(percentGrowth - growthSummary.Average) / growthSummary.StandardDeviation;

            if (percentGrowthWeight < 1.41) // Chebyshev's Theorem
                continue;

            var desc = $"Popularity of {stat.ApplicationType} projects has " +
                percentGrowth switch
                {
                    > 0 => $"grown {percentGrowth:0}%",
                    0 => "remained exactly the same",
                    _ => $"fallen {-1 * percentGrowth:0}%"
                } +
                $" over the last year.";

            yield return new ProjectInsightDto
            {
                Description = desc,                    
                Interestingness = percentGrowthWeight
            };
        }
    }
}
public class ProjectApplicationPopularityStatistic
{
    public required string ApplicationType { get; init; }
    public required int NumberOfProjectsCreatedThisYear { get; init; }
    public required int NumberOfProjectsCreatedLastYear { get; init; }

    public int NumberOfProjectsCreatedDiff => NumberOfProjectsCreatedThisYear - NumberOfProjectsCreatedLastYear;
    public double PercentGrowth => NumberOfProjectsCreatedDiff / (double)NumberOfProjectsCreatedLastYear;
}

public class DataSummary
{
    public double Average { get; }
    public double StandardDeviation { get; }
    public double Sum { get; }

    public DataSummary(IReadOnlyCollection<double> values)
    {
        Average = values.Average();
        StandardDeviation = Math.Sqrt(values.Average(v => Math.Pow(v - Average, 2)));
        Sum = values.Sum();
    }
}
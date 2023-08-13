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
        var today = DateTime.Today.AddMonths(-1);
        var thisYear = today.AddYears(-1);
        var lastYear = thisYear.AddYears(-1);

        var stats = await _dbContext.ProjectApplications
            .Select(a => new ProjectApplicationPopularityStatistic
            {
                ApplicationType = a.Name,
                NumberOfProjectsCreatedThisYear = a.TaggedProjects.Count(p => p.Project.CreatedOn >= thisYear && p.Project.CreatedOn < today),
                NumberOfProjectsCreatedLastYear = a.TaggedProjects.Count(p => p.Project.CreatedOn >= lastYear && p.Project.CreatedOn < thisYear)
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var results = GetGrowthInsights(stats)
            .OrderByDescending(x => x.Interestingness)
            .ToArray();

        return results;
    }

    private static IEnumerable<ProjectInsightDto> GetGrowthInsights(IReadOnlyCollection<ProjectApplicationPopularityStatistic> stats)
    {
        var filteredStats = stats
            .Where(x => x.NumberOfProjectsCreatedThisYear != 0 && x.NumberOfProjectsCreatedLastYear != 0)
            .ToList();

        var growthSummary = new DataSummary(filteredStats.Select(x => x.PercentGrowth * 100.0).ToList());

        foreach (var stat in filteredStats)
        {
            var percentGrowth = 100.0 * stat.PercentGrowth;
            var percentGrowthWeight = Math.Abs(percentGrowth - growthSummary.Average) / growthSummary.StandardDeviation;

            if (percentGrowthWeight < 1.41) // Chebyshev's Theorem
                continue;

            var insightType = GetInsightTypeFromPercentGrowth(percentGrowth);

            var desc = $"Popularity of {stat.ApplicationType} projects has " +
                insightType switch
                {
                    InsightType.Growth or InsightType.SignificantGrowth => $"grown {percentGrowth:0}%",
                    InsightType.Info => "remained exactly the same",
                    _ => $"fallen {-1 * percentGrowth:0}%"
                } +
                $" over the last year.";

            yield return new ProjectInsightDto
            {
                InsightType = insightType,
                Title = "Application Popularity",
                Description = desc,                    
                Interestingness = percentGrowthWeight
            };
        }

        var thisYearCount = stats.Sum(x => x.NumberOfProjectsCreatedThisYear);
        var lastYearCount = stats.Sum(x => x.NumberOfProjectsCreatedLastYear);
        var overallDiff = thisYearCount - lastYearCount;
        var overallGrowth = (thisYearCount - lastYearCount) / (double)lastYearCount;
        var overallGrowthInsightType = GetInsightTypeFromPercentGrowth(overallGrowth);

        yield return new ProjectInsightDto
        {
            InsightType = overallGrowthInsightType,
            Title = "New Project Frequency",
            Description = $"There {(overallDiff == 1 ? "was" : "were")} {Math.Abs(overallDiff)} {(overallDiff > 0 ? "more" : "less")} projects created this year than last year.",
            Interestingness = 1.41
        };

        var mostPopularThisYear = stats.MaxBy(x => x.NumberOfProjectsCreatedThisYear);
        var mostPopularLastYear = stats.MaxBy(x => x.NumberOfProjectsCreatedLastYear);

        if (mostPopularThisYear != mostPopularLastYear)
        {
            yield return new ProjectInsightDto
            {
                InsightType = InsightType.Info,
                Title = "Most Popular Application",
                Description = $"The most popular application type has changed from {mostPopularLastYear?.ApplicationType} to {mostPopularThisYear?.ApplicationType} over the last year.",
                Interestingness = 1.5
            };
        }
    }

    private static InsightType GetInsightTypeFromPercentGrowth(double percentGrowth) => percentGrowth switch
    {
        > 50 => InsightType.SignificantGrowth,
        > 0 => InsightType.Growth,
        < -50 => InsightType.SignificantDecrease,
        < 0 => InsightType.Decrease,
        _ => InsightType.Info
    };
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
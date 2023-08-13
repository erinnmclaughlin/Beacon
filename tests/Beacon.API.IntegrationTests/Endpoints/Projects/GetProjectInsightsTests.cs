using Beacon.API.Features.Projects;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class GetProjectInsightsTests
{
    [Fact]
    public void AddMostPopularApplicationTypeInsight_AddsToList_WhenMostPopularApplicationTypeHasChanged()
    {
        var stats = new List<ProjectApplicationPopularityStatistic>
        {
            new()
            {
                ApplicationType = "Application 1",
                NumberOfProjectsCreatedLastYear = 100,
                NumberOfProjectsCreatedThisYear = 125
            },
            new()
            {
                ApplicationType = "Application 2",
                NumberOfProjectsCreatedLastYear = 110,
                NumberOfProjectsCreatedThisYear = 120,
            }
        };

        var insights = new List<ProjectInsightDto>();
        GetProjectInsightsHandler.AddMostPopularApplicationTypeInsight(insights, stats);
        Assert.Single(insights);
    }


    [Fact]
    public void AddMostPopularApplicationTypeInsight_DoesNotAddToList_WhenMostPopularApplicationTypeHasNotChanged()
    {
        var stats = new List<ProjectApplicationPopularityStatistic>
        {
            new()
            {
                ApplicationType = "Application 1",
                NumberOfProjectsCreatedLastYear = 100,
                NumberOfProjectsCreatedThisYear = 120
            },
            new()
            {
                ApplicationType = "Application 2",
                NumberOfProjectsCreatedLastYear = 110,
                NumberOfProjectsCreatedThisYear = 125,
            }
        };

        var insights = new List<ProjectInsightDto>();
        GetProjectInsightsHandler.AddMostPopularApplicationTypeInsight(insights, stats);
        Assert.Empty(insights);
    }

    [Fact]
    public void AddProjectFrequencyInsight_AddsToList_WhenFrequencyHasChanged()
    {
        var stats = new List<ProjectApplicationPopularityStatistic>
        {
            new()
            {
                ApplicationType = "Application 1",
                NumberOfProjectsCreatedLastYear = 100,
                NumberOfProjectsCreatedThisYear = 125
            },
            new()
            {
                ApplicationType = "Application 2",
                NumberOfProjectsCreatedLastYear = 110,
                NumberOfProjectsCreatedThisYear = 120,
            }
        };

        var insights = new List<ProjectInsightDto>();
        GetProjectInsightsHandler.AddProjectFrequencyInsight(insights, stats);
        Assert.Single(insights);
    }

    [Fact]
    public void AddProjectFrequencyInsight_DoesNotAddToList_WhenFrequencyHasNotChanged()
    {
        var stats = new List<ProjectApplicationPopularityStatistic>
        {
            new()
            {
                ApplicationType = "Application 1",
                NumberOfProjectsCreatedLastYear = 100,
                NumberOfProjectsCreatedThisYear = 110
            },
            new()
            {
                ApplicationType = "Application 2",
                NumberOfProjectsCreatedLastYear = 110,
                NumberOfProjectsCreatedThisYear = 100,
            }
        };

        var insights = new List<ProjectInsightDto>();
        GetProjectInsightsHandler.AddProjectFrequencyInsight(insights, stats);
        Assert.Empty(insights);
    }

    [Theory]
    [InlineData(-100, InsightType.SignificantDecrease)]
    [InlineData(-51, InsightType.SignificantDecrease)]
    [InlineData(-50, InsightType.SignificantDecrease)]
    [InlineData(-49, InsightType.Decrease)]
    [InlineData(-1, InsightType.Decrease)]
    [InlineData(0, InsightType.Info)]
    [InlineData(1, InsightType.Growth)]
    [InlineData(49, InsightType.Growth)]
    [InlineData(50, InsightType.SignificantGrowth)]
    [InlineData(51, InsightType.SignificantGrowth)]
    [InlineData(100, InsightType.SignificantGrowth)]
    public void GetInsightTypeFromPercentGrowth_ReturnsExpectedResult(double percentValue, InsightType expected)
    {
        var actual = GetProjectInsightsHandler.GetInsightTypeFromPercentGrowth(percentValue);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DataSummary_ReturnsExpectedResult()
    {
        var data = new[] { 0, 0.5, 1, 1, 1, 13 };
        var sut = new DataSummary(data);

        Assert.Equal(16.5, sut.Sum);
        Assert.Equal(2.75, sut.Average);
        Assert.Equal(4.598, Math.Round(sut.StandardDeviation, 3));
    }
}

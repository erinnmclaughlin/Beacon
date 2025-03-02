using Beacon.API.Features.Projects;
using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class GetProjectInsightsUnitTests(TestFixture testFixture) : TestBase(testFixture)
{
    [Fact]
    public async Task GetStatsReturnsExpectedResult()
    {
        var sessionContext = new SessionContext
        {
            CurrentLab = new CurrentLab { Id = TestData.Lab.Id, MembershipType = LaboratoryMembershipType.Admin, Name = TestData.Lab.Name },
            CurrentUser = null!
        };

        var fixture = Fixture.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ISessionContext>();
                services.AddScoped<ISessionContext>(_ => sessionContext);
            });
        });

        using var scope = fixture.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        var app1 = CreateApplication("Application 1",
            // Last year (Aug 2021 - Aug 2022)
            new DateOnly(2021, 8, 15),
            new DateOnly(2021, 8, 15),
            new DateOnly(2021, 9, 1),
            new DateOnly(2021, 10, 1),
            new DateOnly(2021, 11, 1),
            new DateOnly(2021, 11, 1),
            new DateOnly(2022, 4, 1),
            // This year (Aug 2022 - Aug 2023)
            new DateOnly(2022, 9, 1),
            new DateOnly(2023, 1, 1));

        var app2 = CreateApplication("Application 2",
            new DateOnly(2021, 10, 1));

        var app3 = CreateApplication("Application 3",
            new DateOnly(2023, 2, 1));

        db.ProjectApplications.AddRange(app1, app2, app3);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new GetProjectInsightsHandler(db);
        var result = await sut.GetStatistics(new DateTime(2023, 8, 1), CancellationToken.None);

        var app1Result = result.Single(x => x.ApplicationType == "Application 1");
        Assert.Equal(7, app1Result.NumberOfProjectsCreatedLastYear);
        Assert.Equal(2, app1Result.NumberOfProjectsCreatedThisYear);
    }

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

    private static ProjectApplication CreateApplication(string name, params DateOnly[] dates)
    {
        var application = new ProjectApplication { Name = name };

        foreach (var date in dates.Distinct())
        {
            var count = dates.Count(d => d == date);
            for (var i = 0; i < count; i++)
            {
                application.TaggedProjects.Add(new ProjectApplicationTag
                {
                    Project = CreateProject(date, i + i)
                });
            }
        }

        return application;
    }

    private static Project CreateProject(DateOnly date, int index)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            CustomerName = "Doesn't Matter",
            ProjectCode = new ProjectCode("TST", date.ToString("yyyyMM"), index + 1),
            CreatedById = TestData.AdminUser.Id,
            CreatedOn = date.ToDateTime(TimeOnly.MinValue)
        };
    }
}

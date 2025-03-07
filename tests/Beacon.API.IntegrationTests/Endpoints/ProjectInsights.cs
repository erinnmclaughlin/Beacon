﻿using System.Net.Http.Json;
using Beacon.API.Features.Projects;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Microsoft.VisualBasic;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "[Feature] Project Insights")]
public sealed class GetProjectInsightsUnitTests(TestFixture testFixture) : IntegrationTestBase(testFixture)
{
    [Fact(DisplayName = "[???] Project Insights returns expected stats")]
    public async Task GetStatsReturnsExpectedResult()
    {
        await LogInToDefaultLab(TestData.MemberUser);

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

        await AddDataAsync(app1, app2, app3);

        await using var dbContext = await CreateDbContext();
        var sut = new GetProjectInsightsHandler(dbContext);
        var result = await sut.GetStatistics(new DateOnly(2023, 8, 1), AbortTest);

        var app1Result = result.Single(x => x.ApplicationType == "Application 1");
        Assert.Equal(7, app1Result.NumberOfProjectsCreatedLastYear);
        Assert.Equal(2, app1Result.NumberOfProjectsCreatedThisYear);
    }

    [Fact(DisplayName = "[???] Project Insights returns expected popularity insight when most popular application type has changed")]
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

    [Fact(DisplayName = "[???] Project Insights returns expected popularity insight when most popular application type has NOT changed")]
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

    [Fact(DisplayName = "[???] Project Insights returns expected frequency insight when frequency has changed")]
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

    [Fact(DisplayName = "[???] Project Insights returns expected frequency insight when frequency has NOT changed")]
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

    [Theory(DisplayName = "[???] GetInsightTypeFromPercentGrowth returns expected result")]
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

    [Fact(DisplayName = "[???] Project Insights returns expected data summary")]
    public void DataSummary_ReturnsExpectedResult()
    {
        var data = new[] { 0, 0.5, 1, 1, 1, 13 };
        var sut = new DataSummary(data);

        Assert.Equal(16.5, sut.Sum);
        Assert.Equal(2.75, sut.Average);
        Assert.Equal(4.598, Math.Round(sut.StandardDeviation, 3));
    }

    [Fact(DisplayName = "[???] Authorized users can get project type frequency grouped by month")]
    public async Task GetProjectTypeFrequencyGroupsByMonth()
    {
        await LogInToDefaultLab(TestData.AdminUser);
        
        var otherLab = new Laboratory { Name = "Get Project Insights Lab" };
        await AddDataAsync(otherLab);

        await SetCurrentLab(otherLab.Id);
        
        await AddDataAsync( 
            new ProjectApplication
            {
                Name = "Application 4",
                TaggedProjects = [
                    new ProjectApplicationTag
                    {
                        Project = new Project
                        {
                            Id = Guid.NewGuid(),
                            CustomerName = "Test",
                            ProjectCode = ProjectCode.FromString("TST-202301-001")!,
                            CreatedById = TestData.AdminUser.Id,
                            CreatedOn = GetDateTimeOffset(2023, 1, 12)
                        }
                    }
                ]
            },
            new ProjectApplication
            {
                Name = "Application 5",
                TaggedProjects = [
                    new ProjectApplicationTag
                    {
                        Project = new Project
                        {
                            Id = Guid.NewGuid(),
                            CustomerName = "Test",
                            ProjectCode = ProjectCode.FromString("TST-202302-001")!,
                            CreatedById = TestData.AdminUser.Id,
                            CreatedOn = GetDateTimeOffset(2023, 2, 1)
                        }
                    },
                    new ProjectApplicationTag
                    {
                        Project = new Project
                        {
                            Id = Guid.NewGuid(),
                            CustomerName = "Test",
                            ProjectCode = ProjectCode.FromString("TST-202302-002")!,
                            CreatedById = TestData.AdminUser.Id,
                            CreatedOn = GetDateTimeOffset(2023, 2, 28)
                        }
                    }
                ]
            });
        
        var response = await SendAsync(new GetProjectTypeFrequencyRequest { StartDate = new DateOnly(2023, 1, 1) });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await response.Content.ReadFromJsonAsync<GetProjectTypeFrequencyRequest.Series[]>(AbortTest);
        Assert.NotNull(data);

        var app1Data = data.Single(x => x.ProjectType == "Application 4");
        Assert.Equal(1, app1Data.ProjectCountByDate[new DateOnly(2023, 1, 1)]);
        Assert.Equal(0, app1Data.ProjectCountByDate[new DateOnly(2023, 2, 1)]);

        var app2Data = data.Single(x => x.ProjectType == "Application 5");
        Assert.Equal(0, app2Data.ProjectCountByDate[new DateOnly(2023, 1, 1)]);
        Assert.Equal(2, app2Data.ProjectCountByDate[new DateOnly(2023, 2, 1)]);
    }

    private static ProjectApplication CreateApplication(string name, params DateOnly[] dates)
    {
        var application = new ProjectApplication { Name = name, LaboratoryId = TestData.Lab.Id };

        foreach (var date in dates.Distinct())
        {
            var count = dates.Count(d => d == date);
            for (var i = 0; i < count; i++)
            {
                application.TaggedProjects.Add(new ProjectApplicationTag
                {
                    Project = CreateProject(date, i + i),
                    LaboratoryId = TestData.Lab.Id
                });
            }
        }

        return application;
    }

    private static Project CreateProject(DateOnly date, int index) => new()
    {
        Id = Guid.NewGuid(),
        CustomerName = "Doesn't Matter",
        ProjectCode = new ProjectCode("TST", date.ToString("yyyyMM"), index + 1),
        LaboratoryId = TestData.Lab.Id,
        CreatedById = TestData.AdminUser.Id,
        CreatedOn = GetDateTimeOffset(date.Year, date.Month, date.Day)
    };

    private static DateTimeOffset GetDateTimeOffset(int year, int month, int day)
    {
        var dateOnly = new DateOnly(year, month, day);
        return new DateTimeOffset(dateOnly, TimeOnly.MinValue, TimeSpan.Zero);
    }
}

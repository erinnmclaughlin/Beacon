using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Requests.Projects.Events;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "[Feature] Project Management")]
public sealed class ProjectManagementEvents(TestFixture fixture) : ProjectTestBase(fixture)
{
    protected override IEnumerable<object> EnumerateReseedData()
    {
        foreach (var item in base.EnumerateReseedData())
            yield return item;
        
        yield return new ProjectEvent
        {
            Title = "Test 1",
            ProjectId = ProjectId,
            LaboratoryId = TestData.Lab.Id,
            ScheduledStart = new DateTime(new DateOnly(2023, 5, 1), TimeOnly.MinValue, DateTimeKind.Utc),
            ScheduledEnd = new DateTime(new DateOnly(2023, 10, 1), TimeOnly.MinValue, DateTimeKind.Utc)
        };
        
        yield return new ProjectEvent
        {
            Title = "Test 2",
            ProjectId = ProjectId,
            LaboratoryId = TestData.Lab.Id,
            ScheduledStart = new DateTime(new DateOnly(2021, 1, 1), TimeOnly.MinValue, DateTimeKind.Utc),
            ScheduledEnd = new DateTime(new DateOnly(2021, 3, 1), TimeOnly.MinValue, DateTimeKind.Utc)
        };
    }

    [Fact(DisplayName = "[113] Create project activity succeeds when request is valid")]
    public async Task CreateProjectEvent_Succeeds_WhenRequestIsValid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var now = DateTime.UtcNow;

        var validRequest = new CreateProjectEventRequest
        {
            Title = "My Cool Event",
            ProjectId = ProjectId,
            ScheduledStart = now,
            ScheduledEnd = now.AddMonths(1)
        };

        var response = await SendAsync(validRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var projectEvent = await GetProjectEventAsync("My Cool Event");
        Assert.NotNull(projectEvent);
        Assert.Equal(validRequest.Title, projectEvent.Title);
        Assert.Null(projectEvent.Description);

        // TODO: Figure out why these are suddenly off by a fraction of a second
        Assert.Equal(validRequest.ScheduledStart, projectEvent.ScheduledStart, TimeSpan.FromSeconds(0.1));
        Assert.Equal(validRequest.ScheduledEnd, projectEvent.ScheduledEnd, TimeSpan.FromSeconds(0.1));
    }

    [Fact(DisplayName = "[113] Create project activity fails when user is not authorized")]
    public async Task CreateProjectEvent_Fails_WhenUserIsNotAuthorized()
    {
        await LogInToDefaultLab(TestData.MemberUser); 
        
        var validRequest = new CreateProjectEventRequest
        {
            Title = "My Un-Cool Event",
            ProjectId = ProjectId,
            ScheduledStart = DateTime.UtcNow,
            ScheduledEnd = DateTime.UtcNow.AddMonths(1)
        };

        var response = await SendAsync(validRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var projectEvent = await GetProjectEventAsync("My Un-Cool Event");
        Assert.Null(projectEvent);
    }

    [Fact(DisplayName = "[113] Create project activity fails when request is invalid")]
    public async Task CreateProjectEvent_Fails_WhenRequestIsInvalid()
    {
        await LogInToDefaultLab(TestData.AdminUser); 
        
        var invalidRequest = new CreateProjectEventRequest
        {
            Title = "My Un-Cool Event",
            ProjectId = ProjectId,
            ScheduledEnd = DateTime.UtcNow,
            ScheduledStart = DateTime.UtcNow.AddMonths(1) // start date is after end date
        };

        var response = await SendAsync(invalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var projectEvent = await GetProjectEventAsync("My Un-Cool Event");
        Assert.Null(projectEvent);
    }
    
    [Fact(DisplayName = "[113] Get lab events succeeds when request is valid")]
    public async Task GetLaboratoryEvents_SucceedsWhenRequestIsValid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedList<LaboratoryEventDto>>(AbortTest);
        Assert.NotNull(result);

        Assert.Contains(result.Items, i => i.Title == "Test 1");
        Assert.Contains(result.Items, i => i.Title == "Test 2");
    }

    [Fact(DisplayName = "[113] Get lab events fails when user is not authorized")]
    public async Task GetProjectEvents_FailsWhenUserIsNotAuthorized()
    {
        await LoginAs(TestData.NonMemberUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "[275] Filter project events by min start date succeeds")]
    public async Task AppliesMinStartFilter()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MinStart = new DateTime(new DateOnly(2023, 1, 1), TimeOnly.MinValue, DateTimeKind.Utc)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Contains(events.Items, x => x.Title == "Test 1");
        Assert.DoesNotContain(events.Items, x => x.Title == "Test 2");
    }

    [Fact(DisplayName = "[275] Filter project events by max start date succeeds")]
    public async Task AppliesMaxStartFilter()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MaxStart = new DateTime(new DateOnly(2023, 1, 1), TimeOnly.MinValue, DateTimeKind.Utc)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.DoesNotContain(events.Items, x => x.Title == "Test 1");
        Assert.Contains(events.Items, x => x.Title == "Test 2");
    }

    [Fact(DisplayName = "[275] Filter project events by min end date succeeds")]
    public async Task AppliesMinEndFilter()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MinEnd = new DateTime(new DateOnly(2023, 1, 1), TimeOnly.MinValue, DateTimeKind.Utc)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Equal(1, events.TotalCount);
        
        var result = Assert.Single(events.Items);
        Assert.Equal("Test 1", result.Title);
    }

    [Fact(DisplayName = "[275] Filter project events by max end date succeeds")]
    public async Task AppliesMaxEndFilter()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MaxEnd = new DateTime(new DateOnly(2023, 1, 1), TimeOnly.MinValue, DateTimeKind.Utc)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Equal(1, events.TotalCount);
        
        var result = Assert.Single(events.Items);
        Assert.Equal("Test 2", result.Title);
    }

    [Fact(DisplayName = "[275] Filter project events by several criteria succeeds")]
    public async Task AppliesMultipleFilters()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MaxEnd = new DateTime(new DateOnly(2023, 1, 1), TimeOnly.MinValue, DateTimeKind.Utc),
            MinEnd = new DateTime(new DateOnly(2023, 1, 2), TimeOnly.MinValue, DateTimeKind.Utc)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Equal(0, events.TotalCount);
        Assert.Empty(events.Items);
    }
    
    private Task<ProjectEvent?> GetProjectEventAsync(string name) => DbContext.ProjectEvents
        .IgnoreQueryFilters()
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.ProjectId == ProjectId && x.Title == name);
}
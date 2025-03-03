﻿using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Events;

public sealed class GetLaboratoryEventsTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    protected override IEnumerable<object> EnumerateReseedData()
    {
        foreach (var item in base.EnumerateReseedData())
            yield return item;
        
        yield return new ProjectEvent
        {
            Title = "Test",
            ProjectId = ProjectId,
            LaboratoryId = TestData.Lab.Id,
            ScheduledStart = new DateTime(2023, 5, 1),
            ScheduledEnd = new DateTime(2023, 10, 1)
        };
        
        yield return new ProjectEvent
        {
            Title = "Test 2",
            ProjectId = ProjectId,
            LaboratoryId = TestData.Lab.Id,
            ScheduledStart = new DateTime(2021, 1, 1),
            ScheduledEnd = new DateTime(2021, 3, 1)
        };
    }
    
    [Fact(DisplayName = "[113] Get lab events succeeds when request is valid")]
    public async Task GetLaboratoryEvents_SucceedsWhenRequestIsValid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedList<LaboratoryEventDto>>(AbortTest);
        Assert.NotNull(result);

        Assert.Contains(result.Items, i => i.Title == "Test");
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
            MinStart = new DateTime(2023, 1, 1)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Equal(1, events.TotalCount);
        
        var result = Assert.Single(events.Items);
        Assert.Equal("Test", result.Title);
    }

    [Fact(DisplayName = "[275] Filter project events by max start date succeeds")]
    public async Task AppliesMaxStartFilter()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MaxStart = new DateTime(2023, 1, 1)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Equal(1, events.TotalCount);
        
        var result = Assert.Single(events.Items);
        Assert.Equal("Test 2", result.Title);
    }

    [Fact(DisplayName = "[275] Filter project events by min end date succeeds")]
    public async Task AppliesMinEndFilter()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MinEnd = new DateTime(2023, 1, 1)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Equal(1, events.TotalCount);
        
        var result = Assert.Single(events.Items);
        Assert.Equal("Test", result.Title);
    }

    [Fact(DisplayName = "[275] Filter project events by max end date succeeds")]
    public async Task AppliesMaxEndFilter()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetLaboratoryEventsRequest
        {
            MaxEnd = new DateTime(2023, 1, 1)
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
            MaxEnd = new DateTime(2023, 1, 1),
            MinEnd = new DateTime(2023, 1, 2)
        });
        
        var events = await response.Content.ReadFromJsonAsync<PagedList<ProjectEvent>>(AbortTest);
        Assert.NotNull(events);
        Assert.Equal(0, events.TotalCount);
        Assert.Empty(events.Items);
    }
}

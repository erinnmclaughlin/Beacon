using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Events;

public sealed class LinkInstrumentToProjectEventTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    private Guid ProjectEventId { get; } = new("f9951855-2d00-486e-8f52-ca1b68beebaa");
    private Guid InstrumentId { get; } = new("4921dfd1-57e1-44b6-a3b1-49635bcccd44");

    [Fact(DisplayName = "[022] Link instrument to project event succeeds when request is valid.")]
    public async Task SucceedsWhenRequestIsValid()
    {
        await LogInToDefaultLab(TestData.AnalystUser);

        var request = new LinkInstrumentToProjectEventRequest 
        {
            InstrumentId = InstrumentId,
            ProjectEventId = ProjectEventId 
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var isLinked = await IsInstrumentLinked();
        Assert.True(isLinked);

        await ResetDatabase();
    }

    [Fact(DisplayName = "[022] Link instrument to project event fails when user is not authorized.")]
    public async Task FailsWhenUserIsNotAuthorized()
    {
        await LogInToDefaultLab(TestData.MemberUser);

        var request = new LinkInstrumentToProjectEventRequest
        {
            InstrumentId = InstrumentId,
            ProjectEventId = ProjectEventId
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var isLinked = await IsInstrumentLinked();
        Assert.False(isLinked);
    }

    [Fact(DisplayName = "[022] Link instrument to project event fails when request is invalid.")]
    public async Task FailsWhenRequestIsInvalid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var request = new LinkInstrumentToProjectEventRequest
        {
            InstrumentId = Guid.Empty,
            ProjectEventId = ProjectEventId
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var isLinked = await IsInstrumentLinked();
        Assert.False(isLinked);
    }

    protected override IEnumerable<object> EnumerateReseedData()
    {
        foreach (var item in base.EnumerateReseedData())
            yield return item;
        
        yield return new ProjectEvent
        {
            Id = ProjectEventId,
            Title = "An Event",
            ScheduledStart = DateTime.Now,
            ScheduledEnd = DateTime.Now.AddMonths(1),
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        };
        yield return new LaboratoryInstrument
        {
            Id = InstrumentId,
            SerialNumber = "Any SN",
            InstrumentType = "Any Instrument Type",
            LaboratoryId = TestData.Lab.Id
        };
    }
    
    private Task<bool> IsInstrumentLinked() => DbContext
        .Set<LaboratoryInstrumentUsage>()
        .IgnoreQueryFilters()
        .AnyAsync(x => x.InstrumentId == InstrumentId && x.ProjectEventId == ProjectEventId);
}

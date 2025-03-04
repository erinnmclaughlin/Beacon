using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "[Feature] Project Management")]
public class ProjectManagementInstrumentUsage(TestFixture fixture) : ProjectTestBase(fixture)
{
    private Guid ProjectEventId { get; } = new("f9951855-2d00-486e-8f52-ca1b68beebaa");
    private Guid PrimaryInstrumentId { get; } = new("4921dfd1-57e1-44b6-a3b1-49635bcccd44");
    private Guid SecondaryInstrumentId { get; } = new("daff1407-e99c-4718-a73e-822ed2c128ca");
    
    protected override IEnumerable<object> EnumerateReseedData()
    {
        foreach (var item in base.EnumerateReseedData())
            yield return item;
        
        yield return new ProjectEvent
        {
            Id = ProjectEventId,
            Title = "An Event",
            ScheduledStart = DateTime.UtcNow,
            ScheduledEnd = DateTime.UtcNow.AddMonths(1),
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        };
        
        yield return new LaboratoryInstrument
        {
            Id = PrimaryInstrumentId,
            SerialNumber = "Default 1",
            InstrumentType = "Any Instrument Type",
            LaboratoryId = TestData.Lab.Id
        };

        yield return new LaboratoryInstrument
        {
            Id = SecondaryInstrumentId,
            SerialNumber = "Default 2",
            InstrumentType = "Any Instrument Type",
            LaboratoryId = TestData.Lab.Id
        };
        
        yield return new LaboratoryInstrumentUsage
        {
            InstrumentId = PrimaryInstrumentId,
            LaboratoryId = TestData.Lab.Id,
            ProjectEventId = ProjectEventId
        };
        
        yield return new LaboratoryInstrumentUsage
        {
            InstrumentId = SecondaryInstrumentId,
            LaboratoryId = TestData.Lab.Id,
            ProjectEventId = ProjectEventId
        };
    }

    [Fact(DisplayName = "[022] Link instrument to project event succeeds when request is valid.")]
    public async Task Link_SucceedsWhenRequestIsValid()
    {
        await LogInToDefaultLab(TestData.AnalystUser);
        
        var instrument = new LaboratoryInstrument
        {
            Id = Guid.NewGuid(),
            SerialNumber = "Test Link",
            InstrumentType = "Nothing"
        };

        await AddDataAsync(instrument);
        
        var response = await SendAsync(new LinkInstrumentToProjectEventRequest 
        {
            InstrumentId = instrument.Id,
            ProjectEventId = ProjectEventId 
        });
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.True(await IsInstrumentLinked("Test Link"));
    }

    [Fact(DisplayName = "[022] Link instrument to project event fails when user is not authorized.")]
    public async Task Link_FailsWhenUserIsNotAuthorized()
    {
        await LogInToDefaultLab(TestData.MemberUser);

        var response = await SendAsync(new LinkInstrumentToProjectEventRequest
        {
            InstrumentId = PrimaryInstrumentId,
            ProjectEventId = ProjectEventId
        });
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var isLinked = await IsInstrumentLinked("Default 1");
        Assert.False(isLinked);
    }

    [Fact(DisplayName = "[022] Link instrument to project event fails when request is invalid.")]
    public async Task Link_FailsWhenRequestIsInvalid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var request = new LinkInstrumentToProjectEventRequest
        {
            InstrumentId = Guid.Empty,
            ProjectEventId = ProjectEventId
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact(DisplayName = "[022] Unlink instrument from project event succeeds when request is valid.")]
    public async Task Unlink_SucceedsWhenRequestIsValid()
    {
        await LogInToDefaultLab(TestData.AnalystUser);

        var response = await SendAsync(new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = PrimaryInstrumentId,
            ProjectEventId = ProjectEventId
        });
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(await IsInstrumentLinked("Default 1"));
    }

    [Fact(DisplayName = "[022] Unlink instrument from project event fails when user is unauthorized.")]
    public async Task Unlink_FailsWhenUserIsNotAuthorized()
    {
        await LogInToDefaultLab(TestData.MemberUser);

        var response = await SendAsync(new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = SecondaryInstrumentId,
            ProjectEventId = ProjectEventId
        });
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(await IsInstrumentLinked("Default 2"));
    }

    [Fact(DisplayName = "[022] Unlink instrument from project event fails when request is invalid.")]
    public async Task Unlink_FailsWhenRequestIsInvalid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = Guid.Empty,
            ProjectEventId = ProjectEventId
        });
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.True(await IsInstrumentLinked("Default 2"));
    }
    
    private Task<bool> IsInstrumentLinked(string sn) => DbContext
        .Set<LaboratoryInstrumentUsage>()
        .IgnoreQueryFilters()
        .Where(x => x.ProjectEventId == ProjectEventId)
        .AnyAsync(x => x.Instrument.SerialNumber == sn);
}
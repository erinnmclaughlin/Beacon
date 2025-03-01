using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Events;

public sealed class UnlinkInstrumentFromProjectEventTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    private Guid ProjectEventId { get; } = new("f9951855-2d00-486e-8f52-ca1b68beebaa");
    private Guid InstrumentId { get; } = new("4921dfd1-57e1-44b6-a3b1-49635bcccd44");

    [Fact(DisplayName = "[022] Unlink instrument from project event succeeds when request is valid.")]
    public async Task SucceedsWhenRequestIsValid()
    {
        RunAsAnalyst();

        var response = await SendAsync(new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = InstrumentId,
            ProjectEventId = ProjectEventId
        });
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(await IsInstrumentLinked());
    }

    [Fact(DisplayName = "[022] Unlink instrument from project event fails when user is unauthorized.")]
    public async Task FailsWhenUserIsNotAuthorized()
    {
        RunAsMember();

        var response = await SendAsync(new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = InstrumentId,
            ProjectEventId = ProjectEventId
        });
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(await IsInstrumentLinked());
    }

    [Fact(DisplayName = "[022] Unlink instrument from project event fails when request is invalid.")]
    public async Task FailsWhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = Guid.Empty,
            ProjectEventId = ProjectEventId
        });
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.True(await IsInstrumentLinked());
    }

    private Task<bool> IsInstrumentLinked() => ExecuteDbContextAsync(async db =>
    {
        var usage = db.Set<LaboratoryInstrumentUsage>();
        return await usage.AnyAsync(x => x.InstrumentId == InstrumentId && x.ProjectEventId == ProjectEventId);
    });
    
    protected override IEnumerable<object> EnumerateTestData() => base.EnumerateTestData().Concat([
        new ProjectEvent
        {
            Id = ProjectEventId,
            Title = "An Event",
            ScheduledStart = DateTime.Now,
            ScheduledEnd = DateTime.Now.AddMonths(1),
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        },
        new LaboratoryInstrument
        {
            Id = InstrumentId,
            SerialNumber = "Any SN",
            InstrumentType = "Any Instrument Type",
            LaboratoryId = TestData.Lab.Id
        },
        new LaboratoryInstrumentUsage
        {
            InstrumentId = InstrumentId,
            LaboratoryId = TestData.Lab.Id,
            ProjectEventId = ProjectEventId
        }
    ]);
}

using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Events;

public sealed class UnlinkInstrumentFromProjectEventTests : ProjectTestBase
{
    private Guid ProjectEventId { get; } = new Guid("f9951855-2d00-486e-8f52-ca1b68beebaa");
    private Guid InstrumentId { get; } = new Guid("4921dfd1-57e1-44b6-a3b1-49635bcccd44");

    public UnlinkInstrumentFromProjectEventTests(TestFixture fixture) : base(fixture)
    {
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.ProjectEvents.Add(new ProjectEvent
        {
            Id = ProjectEventId,
            Title = "An Event",
            ScheduledStart = DateTime.Now,
            ScheduledEnd = DateTime.Now.AddMonths(1),
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        });

        db.LaboratoryInstruments.Add(new LaboratoryInstrument
        {
            Id = InstrumentId,
            SerialNumber = "Any SN",
            InstrumentType = "Any Instrument Type",
            LaboratoryId = TestData.Lab.Id
        });

        db.Set<LaboratoryInstrumentUsage>().Add(new LaboratoryInstrumentUsage
        {
            InstrumentId = InstrumentId,
            LaboratoryId = TestData.Lab.Id,
            ProjectEventId = ProjectEventId
        });

        base.AddTestData(db);
    }

    [Fact(DisplayName = "[022] Unlink instrument from project event succeeds when request is valid.")]
    public async Task SucceedsWhenRequestIsValid()
    {
        RunAsAnalyst();

        var request = new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = InstrumentId,
            ProjectEventId = ProjectEventId
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        await ExecuteDbContext(async dbContext =>
        {
            var exists = await dbContext
                .Set<LaboratoryInstrumentUsage>()
                .AnyAsync(x => x.InstrumentId == InstrumentId && x.ProjectEventId == ProjectEventId);

            Assert.False(exists);
        });
    }

    [Fact(DisplayName = "[022] Unlink instrument from project event fails when user is unauthorized.")]
    public async Task FailsWhenUserIsNotAuthorized()
    {
        RunAsMember();

        var request = new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = InstrumentId,
            ProjectEventId = ProjectEventId
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        await ExecuteDbContext(async dbContext =>
        {
            var exists = await dbContext
                .Set<LaboratoryInstrumentUsage>()
                .AnyAsync(x => x.InstrumentId == InstrumentId && x.ProjectEventId == ProjectEventId);

            Assert.True(exists);
        });
    }

    [Fact(DisplayName = "[022] Unlink instrument from project event fails when request is invalid.")]
    public async Task FailsWhenRequestIsInvalid()
    {
        RunAsAdmin();

        var request = new UnlinkInstrumentFromProjectEventRequest
        {
            InstrumentId = Guid.Empty,
            ProjectEventId = ProjectEventId
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        await ExecuteDbContext(async dbContext =>
        {
            var exists = await dbContext
                .Set<LaboratoryInstrumentUsage>()
                .AnyAsync(x => x.InstrumentId == InstrumentId && x.ProjectEventId == ProjectEventId);

            Assert.True(exists);
        });
    }
}

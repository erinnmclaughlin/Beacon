using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

[Trait("Feature", "Project Events")]
public sealed class GetLaboratoryEventsTests : ProjectTestBase
{
    public GetLaboratoryEventsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[113] Get lab events succeedss when request is valid")]
    public async Task GetLaboratoryEvents_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetLaboratoryEventsRequest());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await DeserializeAsync<LaboratoryEventDto[]>(response);
        Assert.NotNull(result);

        var testEvent = Assert.Single(result);
        Assert.Equal("Test", testEvent.Title);
        Assert.Equal(new DateTime(2023, 5, 1), testEvent.ScheduledStart);
        Assert.Equal(new DateTime(2023, 10, 1), testEvent.ScheduledEnd);
    }

    [Fact(DisplayName = "[113] Get lab events fsils when user is not authorized")]
    public async Task GetProjectEvents_FailsWhenUserIsNotAuthorized()
    {
        RunAsNonMember();

        var response = await SendAsync(new GetLaboratoryEventsRequest());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.ProjectEvents.Add(new ProjectEvent
        {
            Title = "Test",
            ProjectId = ProjectId,
            LaboratoryId = TestData.Lab.Id,
            ScheduledStart = new DateTime(2023, 5, 1),
            ScheduledEnd = new DateTime(2023, 10, 1)
        });

        base.AddTestData(db);
    }
}

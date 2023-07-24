using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Events;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Events;

[Trait("Feature", "Project Events")]
public class GetProjectEventsTests : ProjectTestBase
{
    public GetProjectEventsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[113] Get project events succeeds when request is valid")]
    public async Task GetProjectEvents_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetProjectEventsRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await DeserializeAsync<ProjectEventDto[]>(response);
        Assert.NotNull(result);
        
        var testEvent = Assert.Single(result);
        Assert.Equal("Test", testEvent.Title);
        Assert.Equal(new DateTime(2023, 5, 1), testEvent.ScheduledStart);
        Assert.Equal(new DateTime(2023, 10, 1), testEvent.ScheduledEnd);
    }

    [Fact(DisplayName = "[113] Get project events fails when user is not authorized")]
    public async Task GetProjectEvents_FailsWhenUserIsNotAuthorized()
    {
        RunAsNonMember();

        var response = await SendAsync(new GetProjectEventsRequest { ProjectId = ProjectId });
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

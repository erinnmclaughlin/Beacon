using Beacon.Common.Requests.Projects.Events;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Events;

[Trait("Feature", "Project Events")]
public sealed class CreateProjectEventTests : ProjectTestBase
{
    public CreateProjectEventTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[113] Create project activity succeeds when request is valid")]
    public async Task CreateProjectEvent_Succeeds_WhenRequestIsValid()
    {
        RunAsAdmin();

        var validRequest = new CreateProjectEventRequest
        {
            Title = "My Cool Event",
            ProjectId = ProjectId,
            ScheduledStart = DateTime.Now,
            ScheduledEnd = DateTime.Now.AddMonths(1)
        };

        var response = await SendAsync(validRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        ExecuteDbContext(dbContext =>
        {
            var projectEvent = dbContext.ProjectEvents.Single();
            Assert.Equal(validRequest.Title, projectEvent.Title);
            Assert.Null(projectEvent.Description);
            Assert.Equal(validRequest.ScheduledStart, projectEvent.ScheduledStart);
            Assert.Equal(validRequest.ScheduledEnd, projectEvent.ScheduledEnd);
        });
    }

    [Fact(DisplayName = "[113] Create project activity fails when user is not authorized")]
    public async Task CreateProjectEvent_Fails_WhenUserIsNotAuthorized()
    {
        RunAsMember(); 
        
        var validRequest = new CreateProjectEventRequest
        {
            Title = "My Cool Event",
            ProjectId = ProjectId,
            ScheduledStart = DateTime.Now,
            ScheduledEnd = DateTime.Now.AddMonths(1)
        };

        var response = await SendAsync(validRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        ExecuteDbContext(dbContext => Assert.Null(dbContext.ProjectEvents.SingleOrDefault()));
    }

    [Fact(DisplayName = "[113] Create project activity fails when request is invalid")]
    public async Task CreateProjectEvent_Fails_WhenRequestIsInvalid()
    {
        RunAsAdmin(); 
        
        var invalidRequest = new CreateProjectEventRequest
        {
            Title = "My Cool Event",
            ProjectId = ProjectId,
            ScheduledEnd = DateTime.Now,
            ScheduledStart = DateTime.Now.AddMonths(1) // start date is after end date
        };

        var response = await SendAsync(invalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        ExecuteDbContext(dbContext => Assert.Null(dbContext.ProjectEvents.SingleOrDefault()));
    }
}

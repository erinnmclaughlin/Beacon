using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class CompleteProjectTests : ProjectTestBase
{
    public CompleteProjectTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Complete project succeeds when request is valid")]
    public async Task Complete_SuceedsWhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var request = new CompleteProjectRequest { ProjectId = ProjectId };
        var response = await PostAsync("api/projects/complete", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


    }
}

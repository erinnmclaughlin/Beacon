using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class UpdateLeadAnalystTests : ProjectTestBase
{
    public UpdateLeadAnalystTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[014] Assigning lead analyst fails when analyst is not in valid role")]
    public async Task AssigningLeadAnalyst_ShouldFail_WhenAnalystIsNotValid()
    {
        RunAsAdmin();

        var request = new UpdateLeadAnalystRequest
        {
            ProjectId = ProjectId,
            AnalystId = TestData.MemberUser.Id
        };

        var response = await PutAsync($"api/projects/{ProjectId}/analyst", request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        EnsureLeadAnalystIs(null);
    }

    [Fact(DisplayName = "[014] Assigning lead analyst fails when user is not authorized")]
    public async Task AssigningLeadAnalyst_ShouldFail_WhenUserIsNotAuthorized()
    {
        RunAsMember();

        var request = new UpdateLeadAnalystRequest
        {
            ProjectId = ProjectId,
            AnalystId = TestData.AnalystUser.Id
        }; 
        
        var response = await PutAsync($"api/projects/{ProjectId}/analyst", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        EnsureLeadAnalystIs(null);
    }

    [Fact(DisplayName = "[014] Assigning lead analyst succeeds when request is valid")]
    public async Task AssigningLeadAnalyst_ShouldSucceed_WhenRequestIsValid()
    {
        RunAsAnalyst();

        var request = new UpdateLeadAnalystRequest
        {
            ProjectId = ProjectId,
            AnalystId = TestData.AnalystUser.Id
        };

        var response = await PutAsync($"api/projects/{ProjectId}/analyst", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        EnsureLeadAnalystIs(TestData.AnalystUser.Id);
    }

    [Fact(DisplayName = "[014] Unassigning lead analyst succeeds when request is valid")]
    public async Task UnassigningLeadAnalyst_ShouldSucceed_WhenRequestIsValid()
    {
        ExecuteDbContext(db =>
        {
            var project = db.Projects.Single(p => p.Id == ProjectId);
            project.LeadAnalystId = TestData.AdminUser.Id;
            db.SaveChanges();
        });

        RunAsAnalyst();

        var request = new UpdateLeadAnalystRequest
        {
            ProjectId = ProjectId,
            AnalystId = null
        };

        var response = await PutAsync($"api/projects/{ProjectId}/analyst", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        EnsureLeadAnalystIs(null);
    }

    private void EnsureLeadAnalystIs(Guid? id)
    {
        Assert.Equal(id, ExecuteDbContext(db => db.Projects
            .Where(p => p.Id == ProjectId)
            .Select(p => p.LeadAnalystId)
            .SingleOrDefault()));
    }
}

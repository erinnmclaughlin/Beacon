using Beacon.Common.Requests.Projects;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class UpdateLeadAnalystTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact(DisplayName = "[014] Assigning lead analyst fails when analyst is not in valid role")]
    public async Task AssigningLeadAnalyst_ShouldFail_WhenAnalystIsNotValid()
    {
        RunAsAdmin();

        var request = new UpdateLeadAnalystRequest
        {
            ProjectId = ProjectId,
            AnalystId = TestData.MemberUser.Id
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        Assert.Null(await GetProjectLeadAnalystIdAsync());
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

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        Assert.Null(await GetProjectLeadAnalystIdAsync());
    }

    [Fact(DisplayName = "[014] Assigning lead analyst succeeds when request is valid")]
    public async Task AssigningLeadAnalyst_ShouldSucceed_WhenRequestIsValid()
    {
        RunAsAnalyst();

        var response = await SendAsync(new UpdateLeadAnalystRequest
        {
            ProjectId = ProjectId,
            AnalystId = TestData.AnalystUser.Id
        });
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(TestData.AnalystUser.Id, await GetProjectLeadAnalystIdAsync());
    }

    [Fact(DisplayName = "[014] Unassigning lead analyst succeeds when request is valid")]
    public async Task UnassigningLeadAnalyst_ShouldSucceed_WhenRequestIsValid()
    {
        RunAsAnalyst();

        await ExecuteDbContextAsync(async db =>
        {
            var project = await db.Projects.SingleAsync(p => p.Id == ProjectId);
            project.LeadAnalystId = TestData.AdminUser.Id;
            await db.SaveChangesAsync();
        });

        var request = new UpdateLeadAnalystRequest
        {
            ProjectId = ProjectId,
            AnalystId = null
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Null(await GetProjectLeadAnalystIdAsync());
    }

    private Task<Guid?> GetProjectLeadAnalystIdAsync() => ExecuteDbContextAsync(async db =>
    {
        return await db.Projects
            .Where(p => p.Id == ProjectId)
            .Select(p => p.LeadAnalystId)
            .SingleOrDefaultAsync();
    });
}

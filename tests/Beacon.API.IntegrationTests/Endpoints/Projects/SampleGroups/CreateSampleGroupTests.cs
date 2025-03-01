using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.SampleGroups;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.SampleGroups;

[Trait("Feature", "Sample Management")]
public sealed class CreateSampleGroupTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact(DisplayName = "[016] Create project sample group succeeds when request is valid")]
    public async Task CreateSampleGroup_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateSampleGroupRequest
        {
            ProjectId = ProjectId,
            SampleName = "My Sample Group"
        });
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdSampleGroup = await GetSampleGroupAsync();
        Assert.Equal("My Sample Group", createdSampleGroup?.SampleName);
    }

    [Fact(DisplayName = "[016] Create project sample group fails when request is invalid")]
    public async Task CreateSampleGroup_FailsWhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateSampleGroupRequest
        {
            ProjectId = ProjectId,
            SampleName = ""
        });
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Null(await GetSampleGroupAsync());
    }

    [Fact(DisplayName = "[016] Create project sample group fails when user is not authorized")]
    public async Task CreateSampleGroup_FailsWhenUserIsUnauthorized()
    {
        RunAsMember();

        var response = await SendAsync(new CreateSampleGroupRequest
        {
            ProjectId = ProjectId,
            SampleName = "My Sample Group"
        });
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(await GetSampleGroupAsync());
    }
    
    private Task<SampleGroup?> GetSampleGroupAsync() => ExecuteDbContextAsync(async db =>
    {
        return await db.SampleGroups
            .Where(x => x.LaboratoryId == TestData.Lab.Id)
            .AsNoTracking()
            .SingleOrDefaultAsync();
    });
}

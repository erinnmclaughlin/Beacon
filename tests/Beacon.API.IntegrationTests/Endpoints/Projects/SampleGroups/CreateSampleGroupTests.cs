using Beacon.Common.Requests.Projects.SampleGroups;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.SampleGroups;

[Trait("Feature", "Sample Management")]
public sealed class CreateSampleGroupTests : ProjectTestBase
{
    private static CreateSampleGroupRequest SomeValidRequest => new()
    {
        ProjectId = ProjectId,
        SampleName = "My Sample Group"
    };

    private static CreateSampleGroupRequest SomeInvalidRequest => new()
    {
        ProjectId = ProjectId,
        SampleName = ""
    };

    public CreateSampleGroupTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[016] Create project sample group succeeds when request is valid")]
    public async Task CreateSampleGroup_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdSampleGroup = await ExecuteDbContextAsync(async db => await db.SampleGroups.SingleAsync());
        Assert.Equal(ProjectId, createdSampleGroup.ProjectId);
        Assert.Equal(SomeValidRequest.SampleName, createdSampleGroup.SampleName);
    }

    [Fact(DisplayName = "[016] Create project sample group fails when request is invalid")]
    public async Task CreateSampleGroup_FailsWhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeInvalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdSampleGroup = await ExecuteDbContextAsync(async db => await db.SampleGroups.SingleOrDefaultAsync());
        Assert.Null(createdSampleGroup);
    }

    [Fact(DisplayName = "[016] Create project sample group fails when user is not authorized")]
    public async Task CreateSampleGroup_FailsWhenUserIsUnauthorized()
    {
        RunAsMember();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdSampleGroup = await ExecuteDbContextAsync(async db => await db.SampleGroups.SingleOrDefaultAsync());
        Assert.Null(createdSampleGroup);
    }
}

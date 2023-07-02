using Beacon.API.IntegrationTests.Collections;
using Beacon.Common.Requests.Projects.SampleGroups;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.SampleGroups;

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
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync($"api/projects/{ProjectId}/sample-groups", SomeValidRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdSampleGroup = ExecuteDbContext(db => db.SampleGroups.Single());
        Assert.Equal(ProjectId, createdSampleGroup.ProjectId);
        Assert.Equal(SomeValidRequest.SampleName, createdSampleGroup.SampleName);
    }

    [Fact(DisplayName = "[016] Create project sample group fails when request is invalid")]
    public async Task CreateSampleGroup_FailsWhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync($"api/projects/{ProjectId}/sample-groups", SomeInvalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdSampleGroup = ExecuteDbContext(db => db.SampleGroups.SingleOrDefault());
        Assert.Null(createdSampleGroup);
    }

    [Fact(DisplayName = "[016] Create project sample group fails when user is not authorized")]
    public async Task CreateSampleGroup_FailsWhenUserIsUnauthorized()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var response = await PostAsync($"api/projects/{ProjectId}/sample-groups", SomeValidRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdSampleGroup = ExecuteDbContext(db => db.SampleGroups.SingleOrDefault());
        Assert.Null(createdSampleGroup);
    }
}

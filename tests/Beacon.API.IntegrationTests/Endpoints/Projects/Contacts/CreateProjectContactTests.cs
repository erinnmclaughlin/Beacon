using Beacon.API.IntegrationTests.Collections;
using Beacon.Common.Requests.Projects.Contacts;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

public sealed class CreateProjectContactTests : ProjectTestBase
{
    private static CreateProjectContactRequest SomeValidRequest => new()
    {
        ProjectId = ProjectId,
        Name = "Jenny",
        PhoneNumber = "555-867-5309"
    };

    private static CreateProjectContactRequest SomeInvalidRequest => new()
    {
        ProjectId = ProjectId,
        Name = "",
        PhoneNumber = "555-867-5309"
    };

    public CreateProjectContactTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Create project contact succeeds when request is valid")]
    public async Task CreateContact_SucceedsWhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync($"api/projects/{ProjectId}/contacts", SomeValidRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdContact = ExecuteDbContext(db => db.ProjectContacts.Single());
        Assert.Equal(ProjectId, createdContact.ProjectId);
        Assert.Equal(SomeValidRequest.Name, createdContact.Name);
        Assert.Equal(SomeValidRequest.PhoneNumber, createdContact.PhoneNumber);
        Assert.Null(createdContact.EmailAddress);
    }

    [Fact(DisplayName = "Create project contact fails when user is not authorized")]
    public async Task CreateContact_ShouldFail_WhenUserIsNotAuthorized()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var response = await PostAsync($"api/projects/{ProjectId}/contacts", SomeValidRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdContact = ExecuteDbContext(db => db.ProjectContacts.SingleOrDefault());
        Assert.Null(createdContact);
    }

    [Fact(DisplayName = "Create project contact fails when request is invalid")]
    public async Task CreateContact_ShouldFail_WhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync($"api/projects/{ProjectId}/contacts", SomeInvalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdContact = ExecuteDbContext(db => db.ProjectContacts.SingleOrDefault());
        Assert.Null(createdContact);
    }

    [Fact(DisplayName = "Create project contact fails when uri does not match request")]
    public async Task CreateContact_ShouldFail_WhenUriDoesNotMatchRequest()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync($"api/projects/{Guid.NewGuid()}/contacts", SomeValidRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var createdContact = ExecuteDbContext(db => db.ProjectContacts.SingleOrDefault());
        Assert.Null(createdContact);
    }
}

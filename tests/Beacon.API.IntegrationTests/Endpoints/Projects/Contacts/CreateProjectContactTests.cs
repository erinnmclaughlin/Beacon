using Beacon.Common.Requests.Projects.Contacts;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

[Trait("Feature", "Project Management")]
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

    [Fact(DisplayName = "[013] Create contact endpoint returns 422 when request is invalid")]
    public async Task CreateContact_ShouldFail_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeInvalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdContact = ExecuteDbContext(db => db.ProjectContacts.SingleOrDefault());
        Assert.Null(createdContact);
    }

    [Fact(DisplayName = "[013] Create contact endpoint returns 403 when user is not authorized")]
    public async Task CreateContact_ShouldFail_WhenUserIsNotAuthorized()
    {
        RunAsMember();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdContact = ExecuteDbContext(db => db.ProjectContacts.SingleOrDefault());
        Assert.Null(createdContact);
    }

    [Fact(DisplayName = "[013] Create project contact succeeds when request is valid")]
    public async Task CreateContact_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdContact = ExecuteDbContext(db => db.ProjectContacts.Single());
        Assert.Equal(ProjectId, createdContact.ProjectId);
        Assert.Equal(SomeValidRequest.Name, createdContact.Name);
        Assert.Equal(SomeValidRequest.PhoneNumber, createdContact.PhoneNumber);
        Assert.Null(createdContact.EmailAddress);
    }
}

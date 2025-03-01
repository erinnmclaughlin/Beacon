using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

[Trait("Feature", "Project Management")]
public sealed class CreateProjectContactTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact(DisplayName = "[013] Create contact endpoint returns 422 when request is invalid")]
    public async Task CreateContact_ShouldFail_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateProjectContactRequest
        {
            ProjectId = ProjectId,
            Name = "",
            PhoneNumber = "555-867-5309"
        });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdContact = await GetProjectContactAsync();
        Assert.Null(createdContact);
    }

    [Fact(DisplayName = "[013] Create contact endpoint returns 403 when user is not authorized")]
    public async Task CreateContact_ShouldFail_WhenUserIsNotAuthorized()
    {
        RunAsMember();

        var response = await SendAsync(new CreateProjectContactRequest
        {
            ProjectId = ProjectId,
            Name = "Jenny",
            PhoneNumber = "555-867-5309"
        });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdContact = await GetProjectContactAsync();
        Assert.Null(createdContact);
    }

    [Fact(DisplayName = "[013] Create project contact succeeds when request is valid")]
    public async Task CreateContact_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateProjectContactRequest
        {
            ProjectId = ProjectId,
            Name = "Jenny",
            PhoneNumber = "555-867-5309"
        });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdContact = await GetProjectContactAsync();
        Assert.NotNull(createdContact);
        Assert.Equal(ProjectId, createdContact.ProjectId);
        Assert.Equal("Jenny", createdContact.Name);
        Assert.Equal("555-867-5309", createdContact.PhoneNumber);
        Assert.Null(createdContact.EmailAddress);
    }
    
    private Task<ProjectContact?> GetProjectContactAsync() => ExecuteDbContextAsync(async db =>
    {
        return await db.ProjectContacts
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.LaboratoryId == TestData.Lab.Id);
    });
}

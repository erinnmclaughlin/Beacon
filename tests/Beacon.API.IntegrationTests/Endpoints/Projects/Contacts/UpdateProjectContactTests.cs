using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

[Trait("Feature", "Project Management")]
public sealed class UpdateProjectContactTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    private static Guid ContactId { get; } = new("7d6da369-c88b-4ad8-995f-2c6051f6912b");
    private const string OriginalName = "Old name";
    private const string? OriginalEmail = "someone@place.com";
    private const string? OriginalPhone = null;

    [Fact(DisplayName = "[013] Update contact succeeds when request is valid")]
    public async Task UpdateContact_Succeeds_WhenRequestIsValid()
    {
        RunAsAdmin();

        // Send a valid update request:
        var response = await SendAsync(new UpdateProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId,
            Name = "New name",
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        });
        
        // Verify status code:
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify that the contact was updated:
        var updatedContact = await GetContactAsync();
        Assert.Equal("New name", updatedContact.Name);
        Assert.Null(updatedContact.EmailAddress);
        Assert.Equal("800-588-2300", updatedContact.PhoneNumber);
    }

    [Fact(DisplayName = "[013] Update contact endpoint returns 422 when request is invalid")]
    public async Task UpdateContact_Fails_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        // Send an invalid update request:
        var response = await SendAsync(new UpdateProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId,
            Name = "", // contact must have a name
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        });
        
        // Verify status code:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the contact was NOT updated
        var updatedContact = await GetContactAsync();
        Assert.Equal(OriginalName, updatedContact.Name);
        Assert.Equal(OriginalEmail, updatedContact.EmailAddress);
        Assert.Equal(OriginalPhone, updatedContact.PhoneNumber);
    }

    [Fact(DisplayName = "[013] Update contact endpoint returns 403 when user is unauthorized")]
    public async Task UpdateContact_Fails_WhenUserIsUnauthorized()
    {
        // Members cannot manage project contacts
        RunAsMember();

        // Send an otherwise valid update request:
        var response = await SendAsync(new UpdateProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId,
            Name = "New name",
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        });
        
        // Verify status code:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // Verify that the contact was NOT updated
        var updatedContact = await GetContactAsync();
        Assert.Equal(OriginalName, updatedContact.Name);
        Assert.Equal(OriginalEmail, updatedContact.EmailAddress);
        Assert.Equal(OriginalPhone, updatedContact.PhoneNumber);
    }

    private Task<ProjectContact> GetContactAsync() => ExecuteDbContextAsync(async db =>
    {
        return await db.ProjectContacts.AsNoTracking().SingleAsync(x => x.Id == ContactId);
    });
    
    protected override IEnumerable<object> EnumerateTestData() => base.EnumerateTestData().Append(new ProjectContact
    {
        Id = ContactId,
        Name = OriginalName,
        EmailAddress = OriginalEmail,
        PhoneNumber = OriginalPhone,
        LaboratoryId = TestData.Lab.Id,
        ProjectId = ProjectId
    });
}

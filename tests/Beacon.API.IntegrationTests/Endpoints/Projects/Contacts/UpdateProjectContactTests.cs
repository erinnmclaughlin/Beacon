using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

[Trait("Feature", "Project Management")]
public sealed class UpdateProjectContactTests : ProjectTestBase
{
    private static Guid ContactId { get; } = new("7d6da369-c88b-4ad8-995f-2c6051f6912b");
    private const string OriginalName = "Old name";
    private const string? OriginalEmail = "someone@place.com";
    private const string? OriginalPhone = null;

    public UpdateProjectContactTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[013] Update contact succeeds when request is valid")]
    public async Task UpdateContact_Succeeds_WhenRequestIsValid()
    {
        RunAsAdmin();

        var request = new UpdateProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId,
            Name = "New name",
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updatedContact = await ExecuteDbContextAsync(async db => await db.ProjectContacts.SingleAsync(x => x.Id == ContactId));

        Assert.Equal(request.Name, updatedContact.Name);
        Assert.Equal(request.EmailAddress, updatedContact.EmailAddress);
        Assert.Equal(request.PhoneNumber, updatedContact.PhoneNumber);
    }

    [Fact(DisplayName = "[013] Update contact endpoint returns 422 when request is invalid")]
    public async Task UpdateContact_Fails_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var request = new UpdateProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId,
            Name = "", // contact must have a name
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var updatedContact = await ExecuteDbContextAsync(async db => await db.ProjectContacts.SingleAsync(x => x.Id == ContactId));

        Assert.Equal(OriginalName, updatedContact.Name);
        Assert.Equal(OriginalEmail, updatedContact.EmailAddress);
        Assert.Equal(OriginalPhone, updatedContact.PhoneNumber);
    }

    [Fact(DisplayName = "[013] Update contact endpoint returns 403 when user is unauthorized")]
    public async Task UpdateContact_Fails_WhenUserIsUnauthorized()
    {
        RunAsMember();

        var request = new UpdateProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId,
            Name = "New name",
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var updatedContact = await ExecuteDbContextAsync(async db => await db.ProjectContacts.SingleAsync(x => x.Id == ContactId));

        Assert.Equal(OriginalName, updatedContact.Name);
        Assert.Equal(OriginalEmail, updatedContact.EmailAddress);
        Assert.Equal(OriginalPhone, updatedContact.PhoneNumber);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.ProjectContacts.Add(new()
        {
            Id = ContactId,
            Name = OriginalName,
            EmailAddress =  OriginalEmail,
            PhoneNumber = OriginalPhone,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        });

        base.AddTestData(db);
    }
}

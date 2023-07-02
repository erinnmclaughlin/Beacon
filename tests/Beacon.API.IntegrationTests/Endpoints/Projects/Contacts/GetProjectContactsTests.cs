using Beacon.API.IntegrationTests.Collections;
using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

public sealed class GetProjectContactsTests : ProjectTestBase
{
    public GetProjectContactsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[013] Get contacts endpoint returns list of contacts associated with project")]
    public async Task GetProjectContacts_ReturnsExpectedResult()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync($"api/projects/{ProjectId}/contacts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var contacts = await DeserializeAsync<ProjectContactDto[]>(response);
        Assert.NotNull(contacts);
        Assert.Single(contacts);
        Assert.Equal("Main Contact", contacts[0].Name);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        var otherProject = new Project
        { 
            Id = Guid.NewGuid(),
            CreatedById = TestData.AdminUser.Id,
            CustomerName = "Customer",
            LaboratoryId = TestData.Lab.Id,
            ProjectCode = new ProjectCode("IDK", 1)
        };

        otherProject.Contacts.Add(new ProjectContact
        {
            Id = Guid.NewGuid(),
            Name = "Other Contact",
            EmailAddress = null,
            PhoneNumber = null,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = otherProject.Id
        });

        db.Projects.Add(otherProject);
        db.ProjectContacts.Add(new ProjectContact
        {
            Id = Guid.NewGuid(),
            Name = "Main Contact",
            EmailAddress = null,
            PhoneNumber = null,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        });

        base.AddTestData(db);
    }

}

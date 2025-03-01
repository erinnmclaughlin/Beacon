using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Contacts;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

[Trait("Feature", "Project Management")]
public sealed class GetProjectContactsTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact(DisplayName = "[013] Get contacts endpoint returns list of contacts associated with project")]
    public async Task GetProjectContacts_ReturnsExpectedResult()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetProjectContactsRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var contacts = await DeserializeAsync<ProjectContactDto[]>(response);
        Assert.NotNull(contacts);
        Assert.Single(contacts);
        Assert.Equal("Main Contact", contacts[0].Name);
    }

    protected override IEnumerable<object> EnumerateTestData() => base.EnumerateTestData().Concat([
        new ProjectContact
        {
            Id = Guid.NewGuid(),
            Name = "Main Contact",
            EmailAddress = null,
            PhoneNumber = null,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        },
        // adding another project so we can verify that we only get contacts for the specified project
        new Project
        { 
            Id = Guid.NewGuid(),
            CreatedById = TestData.AdminUser.Id,
            CustomerName = "Customer",
            LaboratoryId = TestData.Lab.Id,
            ProjectCode = new ProjectCode("IDK", "202301", 1),
            Contacts = [
                new ProjectContact
                {
                    Id = Guid.NewGuid(),
                    Name = "Other Contact",
                    EmailAddress = null,
                    PhoneNumber = null,
                    LaboratoryId = TestData.Lab.Id
                }
            ]
        }
    ]);
}

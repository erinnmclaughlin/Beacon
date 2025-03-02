using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "[Feature] Project Management")]
public sealed class ProjectManagementContacts(TestFixture fixture) : IntegrationTestBase(fixture)
{
    private static Project DefaultProject => CreateProject(
        id: new Guid("a2871dc3-8746-45ad-bfd8-87e503d397cd"), 
        customerName: "Default Project",
        projectCode: "DFT-202001-001"
    );
    
    private static ProjectContact DefaultProjectContact => CreateContact(
        contactId: new Guid("b3a02a7a-4f6f-404e-b0f5-5008642a1f2e"), 
        contactName: "Default Contact",
        email: "test@test.com"
    );
    
    protected override IEnumerable<object> EnumerateCustomSeedData()
    {
        var project = DefaultProject;
        project.Contacts.Add(DefaultProjectContact);
        yield return project;
    }
    
    [Fact(DisplayName = "[013] Authorized users can view project contacts")]
    public async Task GetProjectContacts_ReturnsExpectedResult()
    {
        // Create a different project that has a contact (we'll use this to verify that it gets filtered out):
        var otherContact = CreateContact("Some Other Contact");
        await AddDataAsync(CreateProject("Some Other Project", "SOP-202001-001", otherContact));
        
        // Log in as a user that has permission to get project contacts:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to get contacts for the default project:
        var response = await SendAsync(new GetProjectContactsRequest { ProjectId = DefaultProject.Id });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the response contains the expected contacts:
        var contacts = await response.Content.ReadFromJsonAsync<ProjectContactDto[]>(TestContext.Current.CancellationToken);
        Assert.NotNull(contacts);
        Assert.Contains(contacts, c => c.Id == DefaultProjectContact.Id);
        Assert.DoesNotContain(contacts, c => c.Id == otherContact.Id);
    }

    [Fact(DisplayName = "[013] Authorized users can create project contacts")]
    public async Task CreateContact_SucceedsWhenRequestIsValid()
    {
        // Log in as a user that has permission to create project contacts:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to create a new contact:
        var response = await SendAsync(new CreateProjectContactRequest
        {
            ProjectId = DefaultProject.Id,
            Name = "Jenny",
            PhoneNumber = "555-867-5309"
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the contact was created with the expected values:
        var contacts = await HttpClient.GetFromJsonAsync(new GetProjectContactsRequest { ProjectId = DefaultProject.Id }, AbortTest);
        Assert.Contains(contacts ?? [], c => c is { Name: "Jenny", PhoneNumber: "555-867-5309" });
    }
    
    [Fact(DisplayName = "[013] Cannot create project contact without a name")]
    public async Task CreateContact_ShouldFail_WhenRequestIsInvalid()
    {
        // Log in as a user that has permission to create project contacts:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to create a contact without a name:
        var response = await SendAsync(new CreateProjectContactRequest
        {
            ProjectId = DefaultProject.Id,
            Name = "",
            PhoneNumber = "555-867-5309"
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the contact was not created:
        var contacts = await HttpClient.GetFromJsonAsync(new GetProjectContactsRequest { ProjectId = DefaultProject.Id }, AbortTest);       
        Assert.Contains(contacts ?? [], c => c.Id == DefaultProjectContact.Id);
        Assert.DoesNotContain(contacts ?? [], c => c is { Name: "", PhoneNumber: "555-867-5309" });
    }

    [Fact(DisplayName = "[013] Unauthorized users cannot create project contacts")]
    public async Task CreateContact_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Log in as a user that does not have permission to create project contacts:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to create a new contact:
        var response = await SendAsync(new CreateProjectContactRequest
        {
            ProjectId = DefaultProject.Id,
            Name = "Should Not Succeed"
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that the contact was not created:
        var contacts = await HttpClient.GetFromJsonAsync(new GetProjectContactsRequest { ProjectId = DefaultProject.Id }, AbortTest);
        Assert.Contains(contacts ?? [], c => c.Id == DefaultProjectContact.Id);
        Assert.DoesNotContain(contacts ?? [], c => c.Name == "Should Not Succeed");
    }
    
    [Fact(DisplayName = "[013] Authorized users can update project contacts")]
    public async Task UpdateContact_Succeeds_WhenRequestIsValid()
    {
        // Log in as a user that has permission to update project contacts:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Send a valid update request:
        var response = await SendAsync(new UpdateProjectContactRequest
        {
            ContactId = DefaultProjectContact.Id,
            ProjectId = DefaultProject.Id,
            Name = "New name",
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        });
        
        // Verify status code:
        response.EnsureSuccessStatusCode();

        // Verify that the contact was updated:
        var updatedContact = await GetDefaultProjectContact();
        Assert.Equal("New name", updatedContact?.Name);
        Assert.Null(updatedContact?.EmailAddress);
        Assert.Equal("800-588-2300", updatedContact?.PhoneNumber);

        // Reset the database:
        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[013] Cannot update project contact with empty name")]
    public async Task UpdateContact_Fails_WhenRequestIsInvalid()
    {
        // Log in as a user that has permission to update project contacts:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Send an invalid update request:
        var response = await SendAsync(new UpdateProjectContactRequest
        {
            ContactId = DefaultProjectContact.Id,
            ProjectId = DefaultProject.Id,
            Name = "", // contact must have a name
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        });
        
        // Verify status code:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the contact was NOT updated
        var updatedContact = await GetDefaultProjectContact();
        Assert.Equal(DefaultProjectContact.Name, updatedContact?.Name);
        Assert.Equal(DefaultProjectContact.EmailAddress, updatedContact?.EmailAddress);
        Assert.Equal(DefaultProjectContact.PhoneNumber, updatedContact?.PhoneNumber);
    }
    
    [Fact(DisplayName = "[013] Unauthorized users cannot update project contacts")]
    public async Task UpdateContact_Fails_WhenUserIsUnauthorized()
    {
        // Log in as a user that does not have permission to update project contacts:
        await LogInToDefaultLab(TestData.MemberUser);

        // Send an otherwise valid update request:
        var response = await SendAsync(new UpdateProjectContactRequest
        {
            ContactId = DefaultProjectContact.Id,
            ProjectId = DefaultProject.Id,
            Name = "New name",
            EmailAddress = null,
            PhoneNumber = "800-588-2300"
        });
        
        // Verify status code:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // Verify that the contact was NOT updated
        var updatedContact = await GetDefaultProjectContact();
        Assert.Equal(DefaultProjectContact.Name, updatedContact?.Name);
        Assert.Equal(DefaultProjectContact.EmailAddress, updatedContact?.EmailAddress);
        Assert.Equal(DefaultProjectContact.PhoneNumber, updatedContact?.PhoneNumber);
    }
    
    [Fact(DisplayName = "[013] Authorized users can delete project contacts")]
    public async Task DeleteContact_Succeeds_WhenRequestIsValid()
    {
        // Log in as a user that has permission to delete project contacts:
        await LogInToDefaultLab(TestData.AdminUser);
        
        // Send a valid delete request:
        var response = await SendAsync(new DeleteProjectContactRequest
        {
            ContactId = DefaultProjectContact.Id,
            ProjectId = DefaultProject.Id
        });

        // Verify status code:
        response.EnsureSuccessStatusCode();
        
        // Verify that the contact was deleted:
        Assert.Null(await GetDefaultProjectContact());
        
        // Reset the database:
        ShouldResetDatabase = true;
    }

    [Fact(DisplayName = "[013] Unauthorized users cannot delete project contacts")]
    public async Task DeleteContact_FailsWhenUserIsNotAuthorized()
    {
        // Log in as a user that does not have permission to delete project contacts:
        await LogInToDefaultLab(TestData.MemberUser);

        // Send an otherwise valid request to delete a contact:
        var response = await SendAsync(new DeleteProjectContactRequest
        {
            ContactId = DefaultProjectContact.Id,
            ProjectId = DefaultProject.Id
        });

        // Verify status code:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that the contact was NOT deleted:
        Assert.NotNull(await GetDefaultProjectContact());
    }
    
    private static Project CreateProject(string customerName, string projectCode, params ProjectContact[] contacts) 
        => CreateProject(Guid.NewGuid(), customerName, projectCode, contacts);
    private static Project CreateProject(Guid id, string customerName, string projectCode, params ProjectContact[] contacts) => new()
    {
        Id = id,
        CustomerName = customerName,
        ProjectCode = ProjectCode.FromString(projectCode)!,
        ProjectStatus = ProjectStatus.Active,
        CreatedById = TestData.AdminUser.Id,
        LaboratoryId = TestData.Lab.Id,
        Contacts = contacts.ToList()
    };
    private static ProjectContact CreateContact(string contactName) => CreateContact(Guid.NewGuid(), contactName);
    private static ProjectContact CreateContact(Guid contactId, string contactName, string? email = null) => new()
    {
        Id = contactId,
        LaboratoryId = TestData.Lab.Id,
        Name = contactName,
        EmailAddress = email,
        PhoneNumber = null
    };
    
    private Task<ProjectContact?> GetDefaultProjectContact() => DbContext.ProjectContacts
        .IgnoreQueryFilters()
        .AsNoTracking()
        .SingleOrDefaultAsync(c => c.Id == DefaultProjectContact.Id);
}
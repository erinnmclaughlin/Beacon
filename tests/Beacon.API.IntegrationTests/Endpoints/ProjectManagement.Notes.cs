using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Notes;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "[Feature] Project Management")]
public sealed class ProjectManagementNotes(TestFixture fixture) : IntegrationTestBase(fixture)
{
    private static Project DefaultProject => CreateProject(
        id: new Guid("a2871dc3-8746-45ad-bfd8-87e503d397cd"), 
        customerName: "Default Project",
        projectCode: "DFT-202001-001"
    );
    
    private static ProjectNote DefaultProjectNote => CreateNote(
        noteId: new Guid("b3a02a7a-4f6f-404e-b0f5-5008642a1f2e"), 
        content: "Default Note",
        createdAt: DateTime.UtcNow.AddDays(-1)
    );
    
    protected override IEnumerable<object> EnumerateReseedData()
    {
        var project = DefaultProject;
        project.Notes.Add(DefaultProjectNote);
        yield return project;
    }
    
    [Fact(DisplayName = "[013] Authorized users can view project notes")]
    public async Task GetProjectNotes_ReturnsExpectedResult()
    {
        // Create a different project that has a note (we'll use this to verify that it gets filtered out):
        var otherNote = CreateNote("Some Other Note");
        var otherProject = CreateProject("Some Other Project", "SOP-202001-001");
        otherProject.Notes.Add(otherNote);
        await AddDataAsync(otherProject);
        
        // Log in as a user that has permission to get project notes:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to get notes for the default project:
        var response = await SendAsync(new GetProjectNotesRequest { ProjectId = DefaultProject.Id });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the response contains the expected notes:
        var notes = await response.Content.ReadFromJsonAsync<ProjectNoteDto[]>(TestContext.Current.CancellationToken);
        Assert.NotNull(notes);
        Assert.Contains(notes, n => n.Id == DefaultProjectNote.Id);
        Assert.DoesNotContain(notes, n => n.Id == otherNote.Id);
    }

    [Fact(DisplayName = "[013] Authorized users can add project notes")]
    public async Task AddNote_SucceedsWhenRequestIsValid()
    {
        // Log in as a user that has permission to add project notes:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to create a new note:
        var response = await SendAsync(new AddProjectNoteRequest
        {
            ProjectId = DefaultProject.Id,
            Content = "New test note"
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the note was created with the expected values:
        var notes = await HttpClient.GetFromJsonAsync(new GetProjectNotesRequest { ProjectId = DefaultProject.Id }, AbortTest);
        Assert.Contains(notes ?? [], n => n.Content == "New test note");
    }
    
    [Fact(DisplayName = "[013] Cannot create project note without content")]
    public async Task AddNote_ShouldFail_WhenRequestIsInvalid()
    {
        // Log in as a user that has permission to add project notes:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to create a note without content:
        var response = await SendAsync(new AddProjectNoteRequest
        {
            ProjectId = DefaultProject.Id,
            Content = ""
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the note was not created:
        var notes = await HttpClient.GetFromJsonAsync(new GetProjectNotesRequest { ProjectId = DefaultProject.Id }, AbortTest);       
        Assert.Contains(notes ?? [], n => n.Id == DefaultProjectNote.Id);
        Assert.DoesNotContain(notes ?? [], n => n.Content == "");
    }

    [Fact(DisplayName = "[013] Cannot get project notes for non-existent project")]
    public async Task GetProjectNotes_ShouldFail_WhenProjectDoesNotExist()
    {
        // Log in as a user that has permission to get project notes:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to get notes for a project that doesn't exist:
        var response = await SendAsync(new GetProjectNotesRequest { ProjectId = Guid.NewGuid() });
        
        // Verify that this fails with a not found status:
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "[013] Cannot add project note to non-existent project")]
    public async Task AddNote_ShouldFail_WhenProjectDoesNotExist()
    {
        // Log in as a user that has permission to add project notes:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to create a note for a project that doesn't exist:
        var response = await SendAsync(new AddProjectNoteRequest
        {
            ProjectId = Guid.NewGuid(),
            Content = "This note should not be created"
        });
        
        // Verify that this fails with a not found status:
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static Project CreateProject(string customerName, string projectCode) 
        => CreateProject(Guid.NewGuid(), customerName, projectCode);
    
    private static Project CreateProject(Guid id, string customerName, string projectCode) => new()
    {
        Id = id,
        CustomerName = customerName,
        ProjectCode = ProjectCode.FromString(projectCode)!,
        ProjectStatus = ProjectStatus.Active,
        CreatedById = TestData.AdminUser.Id,
        LaboratoryId = TestData.Lab.Id,
        Notes = new List<ProjectNote>()
    };
    
    private static ProjectNote CreateNote(string content) => CreateNote(Guid.NewGuid(), content, DateTime.UtcNow);
    
    private static ProjectNote CreateNote(Guid noteId, string content, DateTime createdAt) => new()
    {
        Id = noteId,
        LaboratoryId = TestData.Lab.Id,
        Content = content,
        CreatedAt = createdAt,
        CreatedById = TestData.AdminUser.Id
    };
    
    private Task<ProjectNote?> GetDefaultProjectNote() => DbContext.ProjectNotes
        .IgnoreQueryFilters()
        .AsNoTracking()
        .SingleOrDefaultAsync(n => n.Id == DefaultProjectNote.Id);
} 
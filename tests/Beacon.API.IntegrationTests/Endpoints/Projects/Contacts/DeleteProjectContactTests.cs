﻿using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Contacts;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.Contacts;

[Trait("Feature", "Project Management")]
public sealed class DeleteProjectContactTests : ProjectTestBase
{
    private static Guid ContactId { get; } = new("7d6da369-c88b-4ad8-995f-2c6051f6912b");

    public DeleteProjectContactTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[013] Delete contact succeeds when request is valid")]
    public async Task DeleteContact_Succeeds_WhenRequestIsValid()
    {
        RunAsAdmin(); 
        
        var request = new DeleteProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId
        };

        var response = await SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(await ExecuteDbContextAsync(async db => await db.ProjectContacts.AnyAsync(c => c.Id == ContactId)));
    }

    [Fact(DisplayName = "[013] Delete contact endpoint returns 403 when user is not authorized")]
    public async Task DeleteContact_FailsWhenUserIsNotAuthorized()
    {
        RunAsMember();

        var request = new DeleteProjectContactRequest
        {
            ContactId = ContactId,
            ProjectId = ProjectId
        };

        var response = await SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(await ExecuteDbContextAsync(async db => await db.ProjectContacts.AnyAsync(c => c.Id == ContactId)));
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.ProjectContacts.Add(new ProjectContact
        {
            Id = ContactId,
            Name = "Johnny Depp",
            EmailAddress = null,
            PhoneNumber = null,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = ProjectId
        });

        base.AddTestData(db);
    }
}

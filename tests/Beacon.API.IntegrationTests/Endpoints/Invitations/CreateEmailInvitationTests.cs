﻿using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

public sealed class CreateEmailInvitationTests : TestBase
{
    public CreateEmailInvitationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Inviting user succeeds when request is valid")]
    public async Task InvitingUserSucceedsWhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var request = new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = "newmanager@test.test",
            MembershipType = LaboratoryMembershipType.Manager
        };

        var response = await PostAsync("api/invitations", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        var emailInvitation = await db.InvitationEmails
            .Include(x => x.LaboratoryInvitation)
            .SingleOrDefaultAsync(x => x.LaboratoryInvitation.NewMemberEmailAddress == request.NewMemberEmailAddress);

        Assert.NotNull(emailInvitation);
        Assert.Equal(TestData.Lab.Id, emailInvitation.LaboratoryId);
        Assert.Equal(FakeEmailSendOperation.OperationId, emailInvitation.OperationId);

        Assert.Equal(TestData.AdminUser.Id, emailInvitation.LaboratoryInvitation.CreatedById);
        Assert.Equal(request.MembershipType, emailInvitation.LaboratoryInvitation.MembershipType);
        Assert.Null(emailInvitation.LaboratoryInvitation.AcceptedById);

        db.InvitationEmails.Remove(emailInvitation);
        db.SaveChanges();

        ResetDatabase();
    }

    [Fact(DisplayName = "Inviting user fails when request is not valid")]
    public async Task InvitingUserFailsWhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var request = new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = TestData.MemberUser.EmailAddress, // user already exists
            MembershipType = LaboratoryMembershipType.Manager
        };
        
        var response = await PostAsync("api/invitations", request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>(); 
        
        var emailInvitationExists = await db.InvitationEmails
            .AnyAsync(x => x.LaboratoryInvitation.NewMemberEmailAddress == request.NewMemberEmailAddress);

        Assert.False(emailInvitationExists);
    }

    [Fact(DisplayName = "Inviting user fails when user is unauthorized")]
    public async Task InvitingUserFailsWhenRequestIsUnauthorized()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var request = new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = "newmanager@test.test",
            MembershipType = LaboratoryMembershipType.Manager
        };

        var response = await PostAsync("api/invitations", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}

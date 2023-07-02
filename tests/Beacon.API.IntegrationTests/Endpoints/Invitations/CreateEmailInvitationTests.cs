using Beacon.API.IntegrationTests.Collections;
using Beacon.API.IntegrationTests.Fakes;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

public sealed class CreateEmailInvitationTests : CoreTestBase
{
    public CreateEmailInvitationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[003] Inviting a new user succeeds when request is valid")]
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

        var emailInvitation = ExecuteDbContext(db => db.InvitationEmails.Include(x => x.LaboratoryInvitation).Single());
        Assert.Equal(TestData.Lab.Id, emailInvitation.LaboratoryId);
        Assert.Equal(FakeEmailSendOperation.OperationId, emailInvitation.OperationId);

        Assert.Equal(TestData.AdminUser.Id, emailInvitation.LaboratoryInvitation.CreatedById);
        Assert.Equal(request.MembershipType, emailInvitation.LaboratoryInvitation.MembershipType);
        Assert.Null(emailInvitation.LaboratoryInvitation.AcceptedById);
    }

    [Fact(DisplayName = "[003] Invite new user endpoint returns 422 when request is not valid")]
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

        var emailInvitation = ExecuteDbContext(db => db.InvitationEmails.Include(x => x.LaboratoryInvitation).SingleOrDefault());
        Assert.Null(emailInvitation);
    }

    [Fact(DisplayName = "[003] Invite new user endpoint returns 403 when user is not authorized")]
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

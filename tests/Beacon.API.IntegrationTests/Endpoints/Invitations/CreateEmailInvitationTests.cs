using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

[Trait("Feature", "User Management")]
public sealed class CreateEmailInvitationTests(TestFixture fixture) : TestBase(fixture)
{
    private const string InvitedUserEmail = "newuser@test.test";
    
    [Fact(DisplayName = "[003] Inviting a new user succeeds when request is valid")]
    public async Task InvitingUserSucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = InvitedUserEmail,
            MembershipType = LaboratoryMembershipType.Manager
        });
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var emailInvitation = await GetEmailInvitationAsync(InvitedUserEmail);
        Assert.NotNull(emailInvitation);
        Assert.Equal(FakeEmailSendOperation.OperationId, emailInvitation.OperationId);
        Assert.Equal(TestData.AdminUser.Id, emailInvitation.LaboratoryInvitation.CreatedById);
        Assert.Equal(LaboratoryMembershipType.Manager, emailInvitation.LaboratoryInvitation.MembershipType);
        Assert.Null(emailInvitation.LaboratoryInvitation.AcceptedById);
    }

    [Fact(DisplayName = "[003] Invite new user endpoint returns 422 when request is not valid")]
    public async Task InvitingUserFailsWhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = TestData.MemberUser.EmailAddress, // user already exists
            MembershipType = LaboratoryMembershipType.Manager
        });
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Null(await GetEmailInvitationAsync(TestData.MemberUser.EmailAddress));
    }

    [Fact(DisplayName = "[003] Invite new user endpoint returns 403 when user is not authorized")]
    public async Task InvitingUserFailsWhenRequestIsUnauthorized()
    {
        RunAsMember();

        var response = await SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = InvitedUserEmail,
            MembershipType = LaboratoryMembershipType.Manager
        });
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(await GetEmailInvitationAsync(InvitedUserEmail));
    }

    private Task<InvitationEmail?> GetEmailInvitationAsync(string email) => ExecuteDbContextAsync(async db =>
    {
        return await db.InvitationEmails
            .Include(x => x.LaboratoryInvitation)
            .Where(x => x.LaboratoryId == TestData.Lab.Id)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.LaboratoryInvitation.NewMemberEmailAddress == email);
    });
}

using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

[Trait("Feature", "User Management")]
public sealed class AcceptEmailInvitationTests(TestFixture fixture) : TestBase(fixture)
{
    private static Guid EmailInvitationId { get; } = new("de50d415-3fea-44dc-ab95-e05b86e6bfdc");
    private static User InvitedUser { get; } = TestData.NonMemberUser;
    private static User UninvitedUser { get; } = TestData.MemberUser;

    [Fact(DisplayName = "[003] Accept invitation succeeds when request is valid")]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        SetCurrentUser(InvitedUser);

        var request = new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId };
        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify that the invitation was accepted by the invited user:
        Assert.Equal(InvitedUser.Id, await GetAcceptedByUserId());

        // Verify that the invited user is now a member of the lab, with the expected membership type:
        Assert.Equal(LaboratoryMembershipType.Analyst, await GetMembershipTypeAsync());
    }

    [Fact(DisplayName = "[003] Accept invitation endpoint returns 403 when current user email does not match email invitation")]
    public async Task AcceptInvitation_ShouldFail_WhenRequestIsUnauthorized()
    {
        SetCurrentUser(UninvitedUser); // try to accept as a different user

        var response = await SendAsync(new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that the invitation is still unaccepted:
        Assert.Null(await GetAcceptedByUserId());
        
        // Verify that the invited user was NOT added to the lab:
        Assert.Null(await GetMembershipTypeAsync());
    }

    [Fact(DisplayName = "[003] Accept invitation endpoint returns 422 when invitation has expired")]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        SetCurrentUser(InvitedUser);

        // update invite to be expired
        await ExecuteDbContextAsync(async db =>
        {
            var invite = await db.InvitationEmails.IgnoreQueryFilters().SingleAsync(x => x.Id == EmailInvitationId);
            invite.ExpiresOn = DateTime.UtcNow.AddDays(-1);
            await db.SaveChangesAsync();
        });

        // Attempt to accept the expired invitation:
        var response = await SendAsync(new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId });
        
        // API should return 422 error:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the invitation is still unaccepted:
        Assert.Null(await GetAcceptedByUserId());
        
        // Verify that the invited user was NOT added to the lab:
        Assert.Null(await GetMembershipTypeAsync());
    }

    private Task<Guid?> GetAcceptedByUserId() => ExecuteDbContextAsync(async db =>
    {
        return await db.InvitationEmails
            .IgnoreQueryFilters()
            .Where(x => x.Id == EmailInvitationId)
            .Select(x => x.LaboratoryInvitation.AcceptedById)
            .SingleAsync();
    });

    private Task<LaboratoryMembershipType?> GetMembershipTypeAsync() => ExecuteDbContextAsync(async db =>
    {
        return await db.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id && x.MemberId == TestData.NonMemberUser.Id)
            .Select(x => (LaboratoryMembershipType?)x.MembershipType)
            .SingleOrDefaultAsync();
    });
    
    protected override IEnumerable<object> EnumerateTestData() => base.EnumerateTestData().Append(new Invitation
    {
        Id = Guid.NewGuid(),
        ExpireAfterDays = 10,
        NewMemberEmailAddress = TestData.NonMemberUser.EmailAddress,
        CreatedById = TestData.AdminUser.Id,
        CreatedOn = DateTime.UtcNow,
        MembershipType = LaboratoryMembershipType.Analyst,
        LaboratoryId = TestData.Lab.Id,
        EmailInvitations = [
            new InvitationEmail
            {
                Id = EmailInvitationId,
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                LaboratoryId = TestData.Lab.Id,
                SentOn = DateTime.UtcNow
            }
        ]
    });
}

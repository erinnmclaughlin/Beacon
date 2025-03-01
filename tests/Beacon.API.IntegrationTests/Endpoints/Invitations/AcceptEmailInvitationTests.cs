using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

[Trait("Feature", "User Management")]
public sealed class AcceptEmailInvitationTests : TestBase
{
    private static Guid EmailInvitationId { get; } = new("de50d415-3fea-44dc-ab95-e05b86e6bfdc");

    public AcceptEmailInvitationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[003] Accept invitation succeeds when request is valid")]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        RunAsNonMember();

        var request = new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId };
        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var invitation = await ExecuteDbContextAsync(async db =>
        {
            return await db.InvitationEmails.IgnoreQueryFilters().Include(x => x.LaboratoryInvitation).SingleAsync();
        });
        Assert.Equal(TestData.NonMemberUser.Id, invitation.LaboratoryInvitation.AcceptedById);

        var membership = await ExecuteDbContextAsync(async db =>
        {
            return await db.Memberships.IgnoreQueryFilters().Where(m => m.MemberId == TestData.NonMemberUser.Id).SingleAsync();
        });
        Assert.Equal(LaboratoryMembershipType.Analyst, membership.MembershipType);
    }

    [Fact(DisplayName = "[003] Accept invitation endpoint returns 403 when current user email does not match email invitation")]
    public async Task AcceptInvitation_ShouldFail_WhenRequestIsUnauthorized()
    {
        RunAsMember(); // try to accept as a different user

        var request = new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId };
        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "[003] Accept invitation endpoint returns 422 when invitation has expired")]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        // update invite to be expired
        await ExecuteDbContextAsync(async db =>
        {
            var invite = await db.InvitationEmails.IgnoreQueryFilters().SingleAsync();
            invite.LaboratoryId = TestData.Lab.Id;
            invite.ExpiresOn = DateTime.UtcNow.AddDays(-1);
            await db.SaveChangesAsync();
        });

        RunAsNonMember();

        var request = new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId };
        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var membership = await ExecuteDbContextAsync(async db =>
        {
            return await db.Memberships.IgnoreQueryFilters().SingleOrDefaultAsync(m => m.MemberId == TestData.NonMemberUser.Id);
        });
        
        Assert.Null(membership);
    }

    protected override void AddTestData(BeaconDbContext dbContext)
    {
        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            ExpireAfterDays = 10,
            NewMemberEmailAddress = TestData.NonMemberUser.EmailAddress,
            CreatedById = TestData.AdminUser.Id,
            CreatedOn = DateTime.UtcNow,
            MembershipType = LaboratoryMembershipType.Analyst,
            LaboratoryId = TestData.Lab.Id
        };

        var emailInvitation = new InvitationEmail
        {
            Id = EmailInvitationId,
            ExpiresOn = DateTime.UtcNow.AddDays(10),
            LaboratoryId = TestData.Lab.Id,
            SentOn = DateTime.UtcNow,
            LaboratoryInvitationId = invitation.Id
        };

        dbContext.Invitations.Add(invitation);
        dbContext.InvitationEmails.Add(emailInvitation);
        base.AddTestData(dbContext);
    }
}

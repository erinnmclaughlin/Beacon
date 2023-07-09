using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

[Trait("Feature", "User Management")]
public sealed class AcceptEmailInvitationTests : TestBase
{
    private static Guid EmailInvitationId { get; } = new Guid("de50d415-3fea-44dc-ab95-e05b86e6bfdc");

    public AcceptEmailInvitationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[003] Accept invitation succeeds when request is valid")]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.NonMemberUser.Id);

        var response = await GetAsync($"api/invitations/{EmailInvitationId}/accept");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var invitation = ExecuteDbContext(db => db.InvitationEmails.Include(x => x.LaboratoryInvitation).Single());
        Assert.Equal(TestData.NonMemberUser.Id, invitation.LaboratoryInvitation.AcceptedById);

        var membership = ExecuteDbContext(db => db.Memberships.Single(m => m.MemberId == TestData.NonMemberUser.Id));
        Assert.Equal(LaboratoryMembershipType.Analyst, membership.MembershipType);
    }

    [Fact(DisplayName = "[003] Accept invitation endpoint returns 403 when current user email does not match email invitation")]
    public async Task AcceptInvitation_ShouldFail_WhenRequestIsUnauthorized()
    {
        SetCurrentUser(TestData.MemberUser.Id); // try to accept as a different user

        var response = await GetAsync($"api/invitations/{EmailInvitationId}/accept");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "[003] Accept invitation endpoint returns 422 when invitation has expired")]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        // update invite to be expired
        ExecuteDbContext(db =>
        {
            var invite = db.InvitationEmails.Single();
            invite.ExpiresOn = DateTime.UtcNow.AddDays(-1);
            db.SaveChanges();
        });

        SetCurrentUser(TestData.NonMemberUser.Id);

        var response = await GetAsync($"api/invitations/{EmailInvitationId}/accept");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var membership = ExecuteDbContext(db => db.Memberships.SingleOrDefault(m => m.MemberId == TestData.NonMemberUser.Id));
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
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(10),
            LaboratoryId = TestData.Lab.Id,
            SentOn = DateTimeOffset.UtcNow,
            LaboratoryInvitationId = invitation.Id
        };

        dbContext.Invitations.Add(invitation);
        dbContext.InvitationEmails.Add(emailInvitation);
        base.AddTestData(dbContext);
    }
}

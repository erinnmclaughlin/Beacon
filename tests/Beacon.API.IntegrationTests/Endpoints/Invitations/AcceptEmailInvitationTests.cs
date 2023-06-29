using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

public sealed class AcceptEmailInvitationTests : TestBase
{
    private static Guid EmailInvitationId { get; } = Guid.NewGuid();

    public AcceptEmailInvitationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.NonMemberUser.Id);

        var response = await GetAsync($"api/invitations/{EmailInvitationId}/accept");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        var invitation = await db.InvitationEmails
            .Include(e => e.LaboratoryInvitation)
            .SingleAsync(e => e.Id == EmailInvitationId);

        Assert.Equal(TestData.NonMemberUser.Id, invitation.LaboratoryInvitation.AcceptedById);

        var membership = await db.Memberships
            .SingleOrDefaultAsync(m => m.LaboratoryId == TestData.Lab.Id && m.MemberId == TestData.NonMemberUser.Id);

        Assert.NotNull(membership);
        Assert.Equal(LaboratoryMembershipType.Analyst, membership.MembershipType);

        ResetDatabase();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenRequestIsUnauthorized()
    {
        SetCurrentUser(TestData.MemberUser.Id); // try to accept as a different user

        var response = await GetAsync($"api/invitations/{EmailInvitationId}/accept");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        ResetDatabase();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        using (var scope = _fixture.Services.CreateScope())
        {
            // update invite to be expired
            var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            var invite = await db.InvitationEmails.SingleAsync();
            invite.ExpiresOn = DateTime.UtcNow.AddDays(-1);
            await db.SaveChangesAsync();
        }

        SetCurrentUser(TestData.NonMemberUser.Id);

        var response = await GetAsync($"api/invitations/{EmailInvitationId}/accept");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        ResetDatabase();
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

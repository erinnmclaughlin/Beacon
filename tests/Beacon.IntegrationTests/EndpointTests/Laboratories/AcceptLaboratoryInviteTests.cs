using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Laboratories.Enums;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class AcceptLaboratoryInviteTests : EndpointTestBase
{
    public AcceptLaboratoryInviteTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        Guid inviteId = Guid.Empty;
        Guid emailId = Guid.Empty;
        Guid labId = Guid.Empty;

        var client = CreateClient(db =>
        {
            (inviteId, emailId, labId) = SeedDbWithEmailInvitation(db, isExpired: false);
        });

        var response = await client.GetAsync($"api/invitations/{inviteId}/accept?emailId={emailId}");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        Guid inviteId = Guid.Empty;
        Guid emailId = Guid.Empty;
        Guid labId = Guid.Empty;

        var client = CreateClient(db =>
        {
            (inviteId, emailId, labId) = SeedDbWithEmailInvitation(db, isExpired: true);
        });

        var response = await client.GetAsync($"api/invitations/{inviteId}/accept?emailId={emailId}");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenRequestIsInvalid()
    {
        var response = await CreateClient().GetAsync($"api/invitations/{Guid.NewGuid()}/accept?emailId={Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static (Guid InviteId, Guid EmailId, Guid LabId) SeedDbWithEmailInvitation(BeaconDbContext dbContext, bool isExpired)
    {
        var labAdmin = SeedLabAdmin(dbContext);
        var lab = SeedLab(dbContext, labAdmin);
        var labInvite = SeedInvite(dbContext, labAdmin.Id, lab.Id, isExpired);
        var emailInvite = labInvite.AddEmailInvitation(DateTimeOffset.UtcNow.AddDays(isExpired ? -30 : 0));

        dbContext.SaveChanges();

        return (labInvite.Id, emailInvite.Id, lab.Id);
    }

    private static User SeedLabAdmin(BeaconDbContext dbContext)
    {
        var labAdmin = new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = "someone@something.com",
            DisplayName = "someone",
            HashedPassword = "",
            HashedPasswordSalt = Array.Empty<byte>()
        };

        dbContext.Users.Add(labAdmin);

        return labAdmin;
    }

    private static Laboratory SeedLab(BeaconDbContext dbContext, User labAdmin)
    {
        var laboratory = Laboratory.CreateNew("Fake Lab", labAdmin);
        dbContext.Laboratories.Add(laboratory);
        return laboratory;
    }

    private static Invitation SeedInvite(BeaconDbContext dbContext, Guid adminId, Guid labId, bool isExpired = false)
    {
        var labInvite = new Invitation
        {
            Id = Guid.NewGuid(),
            ExpireAfterDays = 10,
            NewMemberEmailAddress = TestData.DefaultUser.EmailAddress,
            CreatedById = adminId,
            CreatedOn = DateTime.UtcNow.AddDays(-5),
            LaboratoryId = labId,
            MembershipType = LaboratoryMembershipType.Member
        };

        dbContext.Invitations.Add(labInvite);

        return labInvite;
    }
}

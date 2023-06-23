using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Memberships;

namespace Beacon.IntegrationTests.EndpointTests.Invitations;

[Collection("InvitationTests"), CollectionDefinition("InvitationTests", DisableParallelization = true)]
public class AcceptEmailInvitationTests : EndpointTestBase
{
    public AcceptEmailInvitationTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        Guid emailId = Guid.Empty;
        Guid labId = Guid.Empty;

        var client = CreateClient(db =>
        {
            (emailId, labId) = SeedDbWithEmailInvitation(db, isExpired: false);
        });

        var response = await client.GetAsync($"api/invitations/{emailId}/accept");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        Guid emailId = Guid.Empty;
        Guid labId = Guid.Empty;

        var client = CreateClient(db =>
        {
            (emailId, labId) = SeedDbWithEmailInvitation(db, isExpired: true);
        });

        var response = await client.GetAsync($"api/invitations/{emailId}/accept");
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
    private static (Guid EmailId, Guid LabId) SeedDbWithEmailInvitation(BeaconDbContext dbContext, bool isExpired)
    {
        var labAdmin = SeedLabAdmin(dbContext);
        var lab = SeedLab(dbContext, labAdmin);
        var labInvite = SeedInvite(dbContext, labAdmin.Id, lab.Id);
        var emailInvite = labInvite.AddEmailInvitation(DateTimeOffset.UtcNow.AddDays(isExpired ? -30 : 0));

        dbContext.SaveChanges();

        return (emailInvite.Id, lab.Id);
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
        var laboratory = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = "Fake Lab"
        };

        laboratory.AddMember(labAdmin.Id, LaboratoryMembershipType.Admin);

        dbContext.Laboratories.Add(laboratory);
        return laboratory;
    }

    private static Invitation SeedInvite(BeaconDbContext dbContext, Guid adminId, Guid labId)
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

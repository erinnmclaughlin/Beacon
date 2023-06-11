using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Laboratories;
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
            (inviteId, emailId, labId) = SeedDbWithEmailInvitation(db);
        });

        // api should 404 if user is not a member of the given lab
        var getLabDetailsResponse = await client.GetAsync($"api/laboratories/{labId}");
        getLabDetailsResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var response = await client.GetAsync($"api/invitations/{inviteId}/accept?emailId={emailId}");
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var labDetails = await client.GetFromJsonAsync<LaboratoryDetailDto>($"api/laboratories/{labId}", JsonSerializerOptions);
        var labMembers = labDetails?.Members;
        labMembers.Should().Contain(m => m.Id == TestData.DefaultUser.Id);
    }

    private static (Guid InviteId, Guid EmailId, Guid LabId) SeedDbWithEmailInvitation(BeaconDbContext dbContext)
    {
        var labAdmin = SeedLabAdmin(dbContext);
        var lab = SeedLab(dbContext, labAdmin);
        var labInvite = SeedInvite(dbContext, labAdmin.Id, lab.Id);
        var emailInvite = labInvite.AddEmailInvitation();

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
        var laboratory = Laboratory.CreateNew(Guid.NewGuid(), "Fake Lab", labAdmin);
        dbContext.Laboratories.Add(laboratory);
        return laboratory;
    }

    private static LaboratoryInvitation SeedInvite(BeaconDbContext dbContext, Guid adminId, Guid labId)
    {
        var labInvite = new LaboratoryInvitation
        {
            Id = Guid.NewGuid(),
            ExpireAfterDays = 10,
            NewMemberEmailAddress = TestData.DefaultUser.EmailAddress,
            CreatedById = adminId,
            CreatedOn = DateTime.UtcNow.AddDays(-5),
            LaboratoryId = labId,
            MembershipType = LaboratoryMembershipType.Member
        };

        dbContext.LaboratoryInvitations.Add(labInvite);

        return labInvite;
    }
}

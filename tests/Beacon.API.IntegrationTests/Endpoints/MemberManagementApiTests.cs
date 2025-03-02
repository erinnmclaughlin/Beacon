using Beacon.API.Persistence.Entities;
using Beacon.API.Services;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "Member Management")]
public class MemberManagementApiTests(TestFixture fixture) : IntegrationTestBase(fixture)
{
    private static Guid EmailInvitationId { get; } = new("de50d415-3fea-44dc-ab95-e05b86e6bfdc");
    private static User InvitedUser => TestData.NonMemberUser;
    private static User UninvitedUser => new()
    {
        Id = new Guid("6511db25-672c-4aed-84f3-c36aaa65e717"),
        DisplayName = "Somebody",
        EmailAddress = "idk@place.net",
        HashedPassword = new PasswordHasher().Hash("!!somebody", out var salt),
        HashedPasswordSalt = salt
    };
    
    /// <inheritdoc />
    protected override IEnumerable<object> EnumerateSeedData()
    {
        yield return TestData.AdminUser;
        yield return InvitedUser;
        yield return UninvitedUser;
        
        yield return new InvitationEmail
        {
            Id = EmailInvitationId,
            ExpiresOn = DateTime.UtcNow.AddDays(10),
            LaboratoryId = TestData.Lab.Id,
            SentOn = DateTime.UtcNow,
            LaboratoryInvitation = new Invitation
            {
                ExpireAfterDays = 10,
                NewMemberEmailAddress = InvitedUser.EmailAddress,
                CreatedById = TestData.AdminUser.Id,
                CreatedOn = DateTime.UtcNow,
                MembershipType = LaboratoryMembershipType.Analyst,
                LaboratoryId = TestData.Lab.Id,
            }
        };
    }
    
    [Fact(DisplayName = "[003] Accept invitation succeeds when request is valid")]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        // Log in as the invited user:
        await LoginAs(InvitedUser);

        // Attempt to accept the invitation:
        var response = await HttpClient.SendAsync(new AcceptEmailInvitationRequest
        {
            EmailInvitationId = EmailInvitationId
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the invitation was accepted by the invited user:
        Assert.Equal(InvitedUser.Id, await DbContext.InvitationEmails
            .IgnoreQueryFilters()
            .Where(x => x.Id == EmailInvitationId)
            .Select(x => x.LaboratoryInvitation.AcceptedById)
            .SingleOrDefaultAsync());

        // Verify that the invited user is now a member of the lab, with the expected membership type:
        Assert.Equal(LaboratoryMembershipType.Analyst, await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id && x.MemberId == InvitedUser.Id)
            .Select(x => x.MembershipType)
            .SingleOrDefaultAsync());
        
        // We messed with stuff, so reset the db:
        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[003] Accept invitation endpoint returns 403 when current user email does not match email invitation")]
    public async Task AcceptInvitation_ShouldFail_WhenRequestIsUnauthorized()
    {
        // Log in as an uninvited user:
        await LoginAs(UninvitedUser);

        // Attempt to accept the invitation:
        var response = await HttpClient.SendAsync(new AcceptEmailInvitationRequest
        {
            EmailInvitationId = EmailInvitationId
        });

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that the invitation is still unaccepted:
        Assert.Null(await DbContext.InvitationEmails
            .IgnoreQueryFilters()
            .Where(x => x.Id == EmailInvitationId)
            .Select(x => x.LaboratoryInvitation.AcceptedById)
            .SingleOrDefaultAsync());
        
        // Verify that both the invited and uninvited users are not members of the lab:
        Assert.Equal(0, await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id)
            .Where(x => x.MemberId == InvitedUser.Id || x.MemberId == UninvitedUser.Id)
            .CountAsync());
    }
    
    [Fact(DisplayName = "[003] Accept invitation endpoint returns 422 when invitation has expired")]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        // update the invite to be expired
        await DbContext.InvitationEmails.IgnoreQueryFilters()
            .Where(x => x.Id == EmailInvitationId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.ExpiresOn, DateTime.UtcNow.AddDays(-1)));
        
        // Log in as the invited user:
        await LoginAs(InvitedUser);

        // Attempt to accept the expired invitation:
        var response = await HttpClient.SendAsync(new AcceptEmailInvitationRequest
        {
            EmailInvitationId = EmailInvitationId
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the invitation is still unaccepted:
        Assert.Null(await DbContext.InvitationEmails
            .IgnoreQueryFilters()
            .Where(x => x.Id == EmailInvitationId)
            .Select(x => x.LaboratoryInvitation.AcceptedById)
            .SingleOrDefaultAsync());
        
        // Verify that the invited user was NOT added to the lab:
        Assert.Null(await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id && x.MemberId == InvitedUser.Id)
            .SingleOrDefaultAsync());
    }
}

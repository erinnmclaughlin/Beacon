using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence.Entities;
using Beacon.API.Services;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "API")]
[Trait("Category", "Member Management - Invitations")]
public class MemberManagementInvitationsApiTests(TestFixture fixture) : IntegrationTestBase(fixture)
{
    private static Guid EmailInvitationId { get; } = new("de50d415-3fea-44dc-ab95-e05b86e6bfdc");
    private static User InvitedUser => TestData.NonMemberUser;
    
    /// <inheritdoc />
    protected override IEnumerable<object> EnumerateCustomSeedData()
    {
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
    
    [Theory(DisplayName = "[003] Authorized users can invite members")]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Admin)]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Manager)]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Analyst)]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Member)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Manager)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Analyst)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Member)]
    public async Task InvitingUserSucceedsWhenRequestIsValid(LaboratoryMembershipType currentMemberType, LaboratoryMembershipType invitedMemberType)
    {
        // Log in as a user of the specified type:
        var currentUser = GetDefaultUserForMembershipType(currentMemberType);
        await LogInToDefaultLab(currentUser);

        // Generate some random email for the invited user:
        var email = $"{Guid.NewGuid()}@invited.org";
        
        // Attempt to invite the user with the specified membership level:
        var response = await SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = email,
            MembershipType = invitedMemberType
        });

        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the persisted information matches the request:
        var emailInvitation = await GetEmailInvitationAsync(email);
        Assert.NotNull(emailInvitation);
        Assert.Equal(FakeEmailSendOperation.OperationId, emailInvitation.OperationId);
        Assert.Equal(currentUser.Id, emailInvitation.LaboratoryInvitation.CreatedById);
        Assert.Equal(invitedMemberType, emailInvitation.LaboratoryInvitation.MembershipType);
        Assert.Null(emailInvitation.LaboratoryInvitation.AcceptedById);

        // Technically added stuff to the db, but we don't expect this to affect other tests, so to save on performance, don't reset:
        // ShouldResetDatabase = true;
    }
    
    [Theory(DisplayName = "[003] Unauthorized users cannot invite members")]
    // Managers can't invite admins:
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Admin)]
    // Analysts & members can't invite anyone:
    [InlineData(LaboratoryMembershipType.Analyst, LaboratoryMembershipType.Member)]
    [InlineData(LaboratoryMembershipType.Member, LaboratoryMembershipType.Member)]
    public async Task InvitingUserFailsWhenRequestIsUnauthorized(LaboratoryMembershipType currentMemberType, LaboratoryMembershipType invitedMemberType)
    {
        // Log in as a user of the specified type:
        var currentUser = GetDefaultUserForMembershipType(currentMemberType);
        await LogInToDefaultLab(currentUser);

        // Attempt to invite the user with the specified membership level:
        var response = await SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = "jax@mia.com",
            MembershipType = invitedMemberType
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that the invitation was not persisted:
        Assert.Null(await GetEmailInvitationAsync("jax@mia.com"));
    }

    [Fact(DisplayName = "[003] Invitations cannot be created for existing members")]
    public async Task InvitingUserFailsWhenRequestIsInvalid()
    {
        // Log in as a user that has permission to invite users:
        await LogInToDefaultLab(TestData.ManagerUser);

        // Attempt to invite a user that is already a member of the lab:
        var response = await SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = TestData.MemberUser.EmailAddress, // user is already a member
            MembershipType = LaboratoryMembershipType.Manager
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        
        // Verify that the invitation was not persisted:
        Assert.Null(await GetEmailInvitationAsync(TestData.MemberUser.EmailAddress));
    }
    
    [Fact(DisplayName = "[003] Accept invitation succeeds when request is valid")]
    public async Task AcceptInvitation_ShouldSucceed_WhenRequestIsValid()
    {
        // Log in as the invited user:
        await LoginAs(InvitedUser);

        // Attempt to accept the invitation:
        var response = await SendAsync(new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the invitation was accepted by the invited user:
        var invitation = await GetEmailInvitationAsync(InvitedUser.EmailAddress);
        Assert.Equal(InvitedUser.Id, invitation?.LaboratoryInvitation.AcceptedById);

        // Verify that the invited user is now a member of the lab, with the expected membership type:
        Assert.Equal(LaboratoryMembershipType.Analyst, await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id && x.MemberId == InvitedUser.Id)
            .Select(x => x.MembershipType)
            .SingleOrDefaultAsync(AbortTest));
        
        // We messed with stuff, so reset the db:
        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[003] Accept invitation endpoint returns 403 when current user email does not match email invitation")]
    public async Task AcceptInvitation_ShouldFail_WhenRequestIsUnauthorized()
    {
        // Add some rando user to the db:
        var uninvitedUser = new User
        {
            Id = new Guid("6511db25-672c-4aed-84f3-c36aaa65e717"),
            DisplayName = "Somebody",
            EmailAddress = "idk@place.net",
            HashedPassword = new PasswordHasher().Hash("!!somebody", out var salt),
            HashedPasswordSalt = salt
        };
        await AddDataAsync(uninvitedUser);
        
        // Log in as said rando:
        await LoginAs(uninvitedUser);

        // Attempt to accept the invitation:
        var response = await SendAsync(new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId });

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that the invitation is still unaccepted:
        var invitation = await GetEmailInvitationAsync(InvitedUser.EmailAddress);
        Assert.Null(invitation?.LaboratoryInvitation.AcceptedById);
        
        // Verify that both the invited and uninvited users are not members of the lab:
        Assert.Equal(0, await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id)
            .Where(x => x.MemberId == InvitedUser.Id || x.MemberId == uninvitedUser.Id)
            .CountAsync(AbortTest));
    }
    
    [Fact(DisplayName = "[003] Accept invitation endpoint returns 422 when invitation has expired")]
    public async Task AcceptInvitation_ShouldFail_WhenInvitationIsExpired()
    {
        // update the invite to be expired
        await DbContext.InvitationEmails.IgnoreQueryFilters()
            .Where(x => x.Id == EmailInvitationId)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.ExpiresOn, DateTime.UtcNow.AddDays(-1)), AbortTest);
        
        // Log in as the invited user:
        await LoginAs(InvitedUser);

        // Attempt to accept the expired invitation:
        var response = await SendAsync(new AcceptEmailInvitationRequest { EmailInvitationId = EmailInvitationId });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the invitation is still unaccepted:
        var invitation = await GetEmailInvitationAsync(InvitedUser.EmailAddress);
        Assert.Null(invitation?.LaboratoryInvitation.AcceptedById);
        
        // Verify that the invited user was NOT added to the lab:
        Assert.Null(await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id && x.MemberId == InvitedUser.Id)
            .SingleOrDefaultAsync(AbortTest));
        
        // We messed with stuff, so reset the db:
        ShouldResetDatabase = true;
    }
    
    private async Task<InvitationEmail?> GetEmailInvitationAsync(string email) => await DbContext.InvitationEmails
        .IgnoreQueryFilters()
        .Include(x => x.LaboratoryInvitation)
        .Where(x => x.LaboratoryId == TestData.Lab.Id)
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.LaboratoryInvitation.NewMemberEmailAddress == email);
}

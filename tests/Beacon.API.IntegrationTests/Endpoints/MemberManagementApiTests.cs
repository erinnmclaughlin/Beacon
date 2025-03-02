using System.Net.Http.Json;
using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence.Entities;
using Beacon.API.Services;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Requests.Memberships;
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
        yield return TestData.ManagerUser;
        yield return TestData.AnalystUser;
        yield return TestData.MemberUser;
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
    
    [Fact]
    public async Task SucceedsWhenRequestIsValid_ExcludeHistoricAnalysts()
    {
        await LoginAndSetCurrentLab(TestData.AdminUser);
        
        var response = await HttpClient.SendAsync(new GetAnalystsRequest { IncludeHistoricAnalysts = false });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var members = await response.Content.ReadFromJsonAsync<LaboratoryMemberDto[]>();
        Assert.NotNull(members);
        Assert.Contains(members, m => m.Id == TestData.AdminUser.Id);
        Assert.Contains(members, m => m.Id == TestData.ManagerUser.Id);
        Assert.Contains(members, m => m.Id == TestData.AnalystUser.Id);
        Assert.DoesNotContain(members, m => m.Id == TestData.MemberUser.Id);
    }
    
    [Fact]
    public async Task FailsWhenUserIsNotAuthorized()
    {
        await LoginAs(TestData.NonMemberUser);
        var response = await HttpClient.SendAsync(new GetAnalystsRequest());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SucceedsWhenRequestIsValid_IncludeHistoricAnalysts()
    {
        await LoginAndSetCurrentLab(TestData.AdminUser);
        
        var response = await HttpClient.SendAsync(new GetAnalystsRequest { IncludeHistoricAnalysts = true });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var members = await response.Content.ReadFromJsonAsync<LaboratoryMemberDto[]>();
        Assert.NotNull(members);
        Assert.Contains(members, m => m.Id == TestData.AdminUser.Id);
        Assert.Contains(members, m => m.Id == TestData.ManagerUser.Id);
        Assert.Contains(members, m => m.Id == TestData.AnalystUser.Id);
        Assert.DoesNotContain(members, m => m.Id == TestData.MemberUser.Id);
    }

    
    [Fact(DisplayName = "[003] Inviting a new user succeeds when request is valid")]
    public async Task InvitingUserSucceedsWhenRequestIsValid()
    {
        await LoginAndSetCurrentLab(TestData.AdminUser);

        var response = await HttpClient.SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = "tom.nook@crannies.org",
            MembershipType = LaboratoryMembershipType.Manager
        });

        response.EnsureSuccessStatusCode();

        var emailInvitation = await GetEmailInvitationAsync("tom.nook@crannies.org");
        Assert.NotNull(emailInvitation);
        Assert.Equal(FakeEmailSendOperation.OperationId, emailInvitation.OperationId);
        Assert.Equal(TestData.AdminUser.Id, emailInvitation.LaboratoryInvitation.CreatedById);
        Assert.Equal(LaboratoryMembershipType.Manager, emailInvitation.LaboratoryInvitation.MembershipType);
        Assert.Null(emailInvitation.LaboratoryInvitation.AcceptedById);

        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[003] Invite new user endpoint returns 422 when request is not valid")]
    public async Task InvitingUserFailsWhenRequestIsInvalid()
    {
        await LoginAndSetCurrentLab(TestData.AdminUser);

        var response = await HttpClient.SendAsync(new CreateEmailInvitationRequest
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
        await LoginAndSetCurrentLab(TestData.MemberUser);

        var response = await HttpClient.SendAsync(new CreateEmailInvitationRequest
        {
            NewMemberEmailAddress = "jax@mia.com",
            MembershipType = LaboratoryMembershipType.Manager
        });
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(await GetEmailInvitationAsync("jax@mia.com"));
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
        var invitation = await GetEmailInvitationAsync(InvitedUser.EmailAddress);
        Assert.Equal(InvitedUser.Id, invitation?.LaboratoryInvitation.AcceptedById);

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
        var invitation = await GetEmailInvitationAsync(InvitedUser.EmailAddress);
        Assert.Null(invitation?.LaboratoryInvitation.AcceptedById);
        
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
        var invitation = await GetEmailInvitationAsync(InvitedUser.EmailAddress);
        Assert.Null(invitation?.LaboratoryInvitation.AcceptedById);
        
        // Verify that the invited user was NOT added to the lab:
        Assert.Null(await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.LaboratoryId == TestData.Lab.Id && x.MemberId == InvitedUser.Id)
            .SingleOrDefaultAsync());
    }
    
    private async Task<InvitationEmail?> GetEmailInvitationAsync(string email) => await DbContext.InvitationEmails
        .IgnoreQueryFilters()
        .Include(x => x.LaboratoryInvitation)
        .Where(x => x.LaboratoryId == TestData.Lab.Id)
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.LaboratoryInvitation.NewMemberEmailAddress == email);
}

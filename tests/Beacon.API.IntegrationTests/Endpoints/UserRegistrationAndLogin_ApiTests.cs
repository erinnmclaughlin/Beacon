using System.Net.Http.Json;
using Beacon.API.Services;
using Beacon.Common;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "User Registration & Login")]
public sealed class UserRegistrationAndLoginApiTests(TestFixture fixture) : IntegrationTestBase(fixture)
{
    /// <inheritdoc />
    protected override IEnumerable<object> EnumerateSeedData()
    {
        yield return TestData.AdminUser;
    }

    [Fact(DisplayName = "[001] Creating a new account succeeds when request is valid")]
    public async Task Register_SucceedsWhenRequestIsValid()
    {
        // Attempt to register a valid new user:
        var response = await HttpClient.SendAsync(new RegisterRequest
        {
            EmailAddress = "newuser@website.com",
            Password = "!!newuser",
            DisplayName = "New User"
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the persisted information matches the request:
        var user = await DbContext.Users.SingleAsync(x => x.EmailAddress == "newuser@website.com");
        Assert.Equal("New User", user.DisplayName);
        Assert.True(new PasswordHasher().Verify("!!newuser", user.HashedPassword, user.HashedPasswordSalt));
    }
    
    [Fact(DisplayName = "[001] Creating a new account fails when required info isn't entered")]
    public async Task Register_FailsWhenRequiredInformationIsMissing()
    {
        // Attempt to register a new user without an email address or password:
        var response = await HttpClient.SendAsync(new RegisterRequest());
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "[001] Creating a new account fails when email is already in use")]
    public async Task Register_FailsWhenEmailExists()
    {
        // Attempt to register a new user with an existing email address:
        var response = await HttpClient.SendAsync(new RegisterRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "something",
            DisplayName = "something"
        });

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact(DisplayName = "[008] Login succeeds when request is valid")]
    public async Task Login_SucceedsWhenRequestIsValid()
    {
        // Login:
        var response = await LoginAs(TestData.AdminUser);
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }
    
    [Fact(DisplayName = "[008] Login fails when required information is missing")]
    public async Task Login_FailsWhenRequiredInformationIsMissing()
    {
        // Attempt to log in without providing any information:
        var response = await HttpClient.SendAsync(new LoginRequest());
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }
    
    [Fact(DisplayName = "[008] Login fails when user does not exist")]
    public async Task Login_FailsWhenUserDoesNotExist()
    {
        // Attempt to log in with an email address that doesn't exist:
        var response = await HttpClient.SendAsync(new LoginRequest
        {
            EmailAddress = "notreal@doesntexist.com",
            Password = "password123"
        });

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }
    
    [Fact(DisplayName = "[008] Login fails when password is incorrect")]
    public async Task Login_FailsWhenPasswordIsIncorrect()
    {
        // Attempt to log in with an invalid password:
        var response = await HttpClient.SendAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "NOT!!admin"
        });
            
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }
    
    [Fact(DisplayName = "[008] Get current user returns 401 if user is not logged in")]
    public async Task GetCurrentUser_Returns401_WhenNotLoggedIn()
    {
        // Try to get the current session context without logging in:
        var response = await HttpClient.SendAsync(new GetSessionContextRequest());

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact(DisplayName = "[008] Get current user returns logged in user")]
    public async Task GetCurrentUser_ReturnsExpectedResult_WhenUserIsLoggedIn()
    {
        // Log in:
        await LoginAs(TestData.AdminUser);

        // Get the current session context:
        var getSessionResponse = await HttpClient.SendAsync(new GetSessionContextRequest());
        getSessionResponse.EnsureSuccessStatusCode();

        // Verify the session content matches what expect:
        var session = await getSessionResponse.Content.ReadFromJsonAsync<SessionContext>();
        Assert.NotNull(session);
        Assert.Equal(TestData.AdminUser.Id, session.CurrentUser.Id);
        Assert.Equal(TestData.AdminUser.DisplayName, session.CurrentUser.DisplayName);
    }
    
    [Fact(DisplayName = "[008] Logged in user can successfully log out")]
    public async Task LoggedInUserCanSuccessfullyLogOut()
    {
        // Log in:
        await LoginAs(TestData.AdminUser);

        // Verify that I can get the current session context:
        var getSessionResponse = await HttpClient.SendAsync(new GetSessionContextRequest());
        getSessionResponse.EnsureSuccessStatusCode();

        // Logout:
        await HttpClient.SendAsync(new LogoutRequest());

        // Verify that I can no longer get the current session context:
        getSessionResponse = await HttpClient.SendAsync(new GetSessionContextRequest());
        Assert.Equal(HttpStatusCode.Unauthorized, getSessionResponse.StatusCode);
    }
}

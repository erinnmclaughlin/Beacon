using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Requests.Auth;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Trait("Feature", "User Registration & Login")]
public sealed class RegisterTests : TestBase
{
    public RegisterTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[001] Creating a new account succeeds when request is valid")]
    public async Task Register_SucceedsWhenRequestIsValid()
    {
        RunAsAnonymous();

        var request = new RegisterRequest
        {
            EmailAddress = "newuser@website.com",
            Password = "!!newuser",
            DisplayName = "New User"
        };

        var response = await SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var user = ExecuteDbContext(db => db.Users.Single(x => x.EmailAddress == request.EmailAddress));
        Assert.Equal(request.DisplayName, user.DisplayName);
        Assert.True(new PasswordHasher().Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt));
    }

    [Fact(DisplayName = "[001] Creating a new account fails when required info isn't entered")]
    public async Task Register_FailsWhenRequiredInformationIsMissing()
    {
        RunAsAnonymous();

        var response = await SendAsync(new RegisterRequest());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "[001] Creating a new account fails when email is already in use")]
    public async Task Register_FailsWhenEmailExists()
    {
        RunAsAnonymous();

        var response = await SendAsync(new RegisterRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "something",
            DisplayName = "something"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.Users.Add(TestData.AdminUser);
        db.SaveChanges();
    }
}

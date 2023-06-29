using Beacon.API.IntegrationTests.Collections;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Requests.Auth;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class RegisterTests : CoreTestBase
{
    public RegisterTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Register succeeds when request is valid")]
    public async Task Register_SucceedsWhenRequestIsValid()
    {
        var request = new RegisterRequest
        {
            EmailAddress = "newuser@website.com",
            Password = "!!newuser",
            DisplayName = "New User"
        };

        var response = await PostAsync("api/auth/register", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var user = ExecuteDbContext(db => db.Users.Single(x => x.EmailAddress == request.EmailAddress));
        Assert.Equal(request.DisplayName, user.DisplayName);
        Assert.True(new PasswordHasher().Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt));
    }

    [Fact(DisplayName = "Register fails when required information is missing")]
    public async Task Register_FailsWhenRequiredInformationIsMissing()
    {
        var response = await PostAsync("api/auth/register", new RegisterRequest());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Register fails when email is already associated with an account")]
    public async Task Register_FailsWhenEmailExists()
    {
        var response = await PostAsync("api/auth/register", new RegisterRequest
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

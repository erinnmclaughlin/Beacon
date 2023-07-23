using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

[Trait("Feature", "Laboratory Management")]
public sealed class SetCurrentLaboratoryTests : IClassFixture<AuthTestFixture>
{
    private readonly AuthTestFixture _fixture;

    public SetCurrentLaboratoryTests(AuthTestFixture fixture)
    {
        _fixture = fixture;
        AddSeedData();
    }

    [Fact(DisplayName = "[185] Set current lab succeeds when request is valid")]
    public async Task SetCurrentLaboratory_SucceedsWhenRequestIsValid()
    {
        var httpClient = _fixture.CreateClient();

        await LoginRequest.SendAsync(httpClient, new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        var response = await SetCurrentLaboratoryRequest.SendAsync(httpClient, new()
        {
            LaboratoryId = TestData.Lab.Id
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact(DisplayName = "[185] Set current lab fails when user is not a lab member")]
    public async Task SetCurrentLaboratory_FailsWhenUserIsNotAMember()
    {
        var httpClient = _fixture.CreateClient();

        await LoginRequest.SendAsync(httpClient, new LoginRequest
        {
            EmailAddress = TestData.NonMemberUser.EmailAddress,
            Password = "!!nonmember"
        });
        
        var response = await SetCurrentLaboratoryRequest.SendAsync(httpClient, new()
        {
            LaboratoryId = TestData.Lab.Id
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }

    private void AddSeedData()
    {
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        
        if (dbContext.Database.EnsureCreated())
        {
            dbContext.AddRange(
                TestData.AdminUser, 
                TestData.ManagerUser, 
                TestData.AnalystUser,
                TestData.MemberUser, 
                TestData.NonMemberUser,
                TestData.Lab
            );

            dbContext.SaveChanges();
        }
       
    }
}

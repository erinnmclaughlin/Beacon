using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

// This class intentionally does not inherit from TestBase, since TestBase is configured with mocked auth services

[Trait("Feature", "Laboratory Management")]
public sealed class SetCurrentLaboratoryTests(AuthTestFixture fixture) : IClassFixture<AuthTestFixture>, IAsyncLifetime
{
    private readonly AuthTestFixture _fixture = fixture;

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
    
    public async Task InitializeAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        
        if (await dbContext.Database.EnsureCreatedAsync())
        {
            dbContext.AddRange(
                TestData.AdminUser, 
                TestData.ManagerUser, 
                TestData.AnalystUser,
                TestData.MemberUser, 
                TestData.NonMemberUser,
                TestData.Lab
            );

            await dbContext.SaveChangesAsync();
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

public sealed class SetCurrentLaboratoryTests : IClassFixture<AuthTestFixture>
{
    private readonly AuthTestFixture _fixture;

    public SetCurrentLaboratoryTests(AuthTestFixture fixture)
    {
        _fixture = fixture;
        AddSeedData();
    }

    [Fact(DisplayName = "Set current lab succeeds when request is valid")]
    public async Task SetCurrentLaboratory_SucceedsWhenRequestIsValid()
    {
        var httpClient = _fixture.CreateClient();

        await Login(httpClient, new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        var response = await httpClient.PostAsJsonAsync("api/laboratories/current", new SetCurrentLaboratoryRequest
        {
            LaboratoryId = TestData.Lab.Id
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact(DisplayName = "Set current lab fails when user is not a lab member")]
    public async Task SetCurrentLaboratory_FailsWhenUserIsNotAMember()
    {
        var httpClient = _fixture.CreateClient();

        await Login(httpClient, new LoginRequest
        {
            EmailAddress = TestData.NonMemberUser.EmailAddress,
            Password = "!!nonmember"
        });
        
        var response = await httpClient.PostAsJsonAsync("api/laboratories/current", new SetCurrentLaboratoryRequest
        {
            LaboratoryId = TestData.Lab.Id
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }

    private static async Task Login(HttpClient httpClient, LoginRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", request);
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
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

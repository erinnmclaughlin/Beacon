using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Beacon.WebHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class LoginTests : IClassFixture<DbContextFixture>, IClassFixture<WebApplicationFactory<BeaconWebHost>>
{
    private readonly HttpClient _httpClient;

    public LoginTests(DbContextFixture dbContextFixture, WebApplicationFactory<BeaconWebHost> factory)
    {
        _httpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<BeaconDbContext>>();
                services.RemoveAll<BeaconDbContext>();

                services.AddSingleton(_ => dbContextFixture);
                services.AddScoped(sp => sp.GetRequiredService<DbContextFixture>().CreateDbContext());
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Login_FailsWhenRequiredInformationIsMissing()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest 
        {
            EmailAddress = "", 
            Password = "" 
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Login_FailsWhenPasswordIsIncorrect()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "NOT!!admin"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
}

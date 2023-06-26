using Beacon.API.Persistence;
using Beacon.WebHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Beacon.API.IntegrationTests.Endpoints;

public abstract class TestBase
{
    protected readonly HttpClient _httpClient;

    public TestBase(DbContextFixture dbContextFixture, WebApplicationFactory<BeaconWebHost> factory)
    {
        _httpClient = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<BeaconDbContext>>();
                    services.RemoveAll<BeaconDbContext>();

                    services.AddSingleton(_ => dbContextFixture);
                    services.AddScoped(sp => sp.GetRequiredService<DbContextFixture>().CreateDbContext());
                });
            })
            .CreateClient();
    }
}

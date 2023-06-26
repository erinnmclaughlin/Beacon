using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Beacon.API.IntegrationTests;

public abstract class TestBase
{
    protected readonly WebApplicationFactory<BeaconWebHost> _factory;

    public TestBase(DbContextFixture dbContextFixture, WebApplicationFactory<BeaconWebHost> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<BeaconDbContext>>();
                services.RemoveAll<BeaconDbContext>();

                services.AddSingleton(_ => dbContextFixture);
                services.AddScoped(sp => sp.GetRequiredService<DbContextFixture>().CreateDbContext());
            });
        });
    }
}

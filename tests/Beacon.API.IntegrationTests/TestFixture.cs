using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();
    private Respawner? _respawner;

    public string ConnectionString => _container.GetConnectionString().Replace("master", "Beacon");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase(ConnectionString);
            services.UseMockedCurrentUser();
            services.UseMockedLabContext();
            services.UseFakeEmailService();
        });
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public async Task ResetDatabase()
    {
        var options = new DbContextOptionsBuilder<BeaconDbContext>().UseSqlServer(ConnectionString).Options;
        var dbContext = new BeaconDbContext(options, null!);

        if (_respawner is null)
        {
            await dbContext.Database.EnsureCreatedAsync();
            _respawner = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
            {
                WithReseed = true
            });
        }
        else
        {
            await _respawner.ResetAsync(ConnectionString);
        }
    }
}

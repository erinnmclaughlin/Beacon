using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace Beacon.API.IntegrationTests;

public sealed class AuthTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _container.GetConnectionString().Replace("master", "Beacon");

        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase(connectionString);
            services.UseFakeEmailService();
        });

        builder.ConfigureLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Warning);
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
}
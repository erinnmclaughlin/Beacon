using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Beacon.API.IntegrationTests;

public sealed class AuthTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase();
        });

        builder.ConfigureLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Warning);
        });
    }
}
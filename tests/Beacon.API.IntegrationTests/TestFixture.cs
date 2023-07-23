using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase();
            services.UseMockedCurrentUser();
            services.UseMockedLabContext();
            services.UseFakeEmailService();
        });

        builder.ConfigureLogging(config =>
        {
            config.SetMinimumLevel(LogLevel.Warning);
            config.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        });
    }
}

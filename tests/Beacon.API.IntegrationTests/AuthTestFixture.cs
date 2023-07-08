using Microsoft.AspNetCore.Hosting;

namespace Beacon.API.IntegrationTests;

public sealed class AuthTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase();
            services.SuppressLogging();
        });
    }
}
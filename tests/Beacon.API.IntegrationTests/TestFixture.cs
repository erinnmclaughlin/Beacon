using Microsoft.AspNetCore.Hosting;

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
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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

            services.RemoveAll<ILoggerFactory>();
            services.AddScoped<ILoggerFactory>(_ => new NullLoggerFactory());
        });
    }
}

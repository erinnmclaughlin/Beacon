using Beacon.Common.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Beacon.API.IntegrationTests;

public sealed class AuthTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase();
            services.SuppressLogging();
            services.RemoveAll<ILabContext>();
            services.AddScoped(_ => Mock.Of<ILabContext>());
        });
    }
}
using Microsoft.AspNetCore.Hosting;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture : WebApplicationFactory<BeaconWebHost>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase();
            services.UseMockedCurrentUser();
            services.UseMockedHttpContextAccessor();
            services.UseFakeEmailService();
        });
    }
}

[CollectionDefinition(nameof(AuthTestFixture))]
public class AuthTestFixtureCollection : ICollectionFixture<AuthTestFixture> { }

public sealed class AuthTestFixture : WebApplicationFactory<BeaconWebHost>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase();
        });
    }
}
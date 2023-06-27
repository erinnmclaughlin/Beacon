using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;
using System.Net.Http.Headers;

namespace Beacon.API.IntegrationTests;

public abstract class TestBase : IAsyncLifetime, IClassFixture<ApiFactory>
{
    protected readonly ApiFactory _factory;

    public TestBase(ApiFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<BeaconDbContext>().InitializeForTests();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

public sealed class ApiFactory : WebApplicationFactory<BeaconWebHost>
{
    private readonly DbConnection _dbConnection;

    public ApiFactory()
    {
        _dbConnection = new SqliteConnection("DataSource=:memory:");
        _dbConnection.Open();
    }

    public HttpClient CreateClient(Guid userId, Guid? labId = null)
    {
        var factory = WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .Configure<TestAuthHandlerOptions>(options => options.UserId = userId)
                    .AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");

        if (labId != null)
            client.DefaultRequestHeaders.Add("X-LaboratoryId", labId.Value.ToString());

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BeaconDbContext>>();
            services.RemoveAll<BeaconDbContext>();
            services.AddDbContext<BeaconDbContext>(o => o.UseSqlite(_dbConnection));
        });
    }

    public override ValueTask DisposeAsync()
    {
        _dbConnection.Close();
        _dbConnection.Dispose();
        return base.DisposeAsync();
    }
}

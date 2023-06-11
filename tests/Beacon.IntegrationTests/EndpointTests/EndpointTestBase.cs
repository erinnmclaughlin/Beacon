using Beacon.API.Persistence;
using Beacon.WebHost;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beacon.IntegrationTests.EndpointTests;

public abstract class EndpointTestBase : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;
    private bool _addTestAuthorization;

    protected static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }

    public EndpointTestBase(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
    }

    protected void AddTestAuthorization(bool addAuthorization = true)
    {
        _addTestAuthorization = addAuthorization;
    }

    protected IServiceScope CreateScope()
    {
        return _factory.Services.CreateScope();
    }

    protected HttpClient CreateClient(Action<BeaconDbContext>? dbAction = null)
    {
        var factory = GetWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            db.EnsureSeeded();
            dbAction?.Invoke(db);
        }

        var client = factory.CreateClient();

        if (_addTestAuthorization)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");

        return client;
    }

    private WebApplicationFactory<BeaconWebHost> GetWebApplicationFactory()
    {
        if (!_addTestAuthorization)
        {
            return _factory;
        }

        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });
    }
}

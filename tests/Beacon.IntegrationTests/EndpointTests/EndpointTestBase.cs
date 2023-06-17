using Beacon.API.Persistence;
using Beacon.Common.Laboratories;
using Beacon.WebHost;
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

    private bool IsAuthenticated { get; set; }
    private LaboratoryMembershipType? MembershipType { get; set; }

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

    protected void AddTestAuthorization(LaboratoryMembershipType? membershipType = null)
    {
        IsAuthenticated = true;
        MembershipType = membershipType;
    }

    protected HttpClient CreateClient(Action<BeaconDbContext>? dbAction = null)
    {
        var factory = GetWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

            if (db.Database.EnsureCreated())
                db.SeedWithTestData();

            dbAction?.Invoke(db);
        }

        var client = factory.CreateClient();

        if (IsAuthenticated)
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "TestScheme");

        return client;
    }

    private WebApplicationFactory<BeaconWebHost> GetWebApplicationFactory()
    {
        if (!IsAuthenticated)
        {
            return _factory;
        }

        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .Configure<TestAuthSchemeOptions>(options => options.MembershipType = MembershipType)
                    .AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<TestAuthSchemeOptions, TestAuthHandler>("TestScheme", options => { });
            });
        });
    }
}

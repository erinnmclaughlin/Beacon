using Beacon.WebHost;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[CollectionDefinition(Name)]
public class AuthTests : ICollectionFixture<WebApplicationFactory<BeaconWebHost>>, ICollectionFixture<DbContextFixture>
{
    public const string Name = "Auth Tests";
}

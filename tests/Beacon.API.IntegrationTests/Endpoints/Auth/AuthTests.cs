namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[CollectionDefinition(Name)]
public sealed class AuthTests : ICollectionFixture<WebApplicationFactory<BeaconWebHost>>, ICollectionFixture<DbContextFixture>
{
    public const string Name = "Auth Tests";
}

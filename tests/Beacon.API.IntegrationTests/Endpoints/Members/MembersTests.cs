namespace Beacon.API.IntegrationTests.Endpoints.Members;

[CollectionDefinition(Name)]
public sealed class MembersTests : ICollectionFixture<WebApplicationFactory<BeaconWebHost>>, ICollectionFixture<DbContextFixture>
{
    public const string Name = "Members Tests";
}

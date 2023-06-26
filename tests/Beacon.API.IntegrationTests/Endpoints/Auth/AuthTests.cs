namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[CollectionDefinition(Name)]
public sealed class AuthTests : ICollectionFixture<ApiFactory>
{
    public const string Name = "Auth Tests";
}

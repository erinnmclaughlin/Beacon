namespace Beacon.API.IntegrationTests;

[CollectionDefinition(nameof(ContainerFixtureCollection))]
public class ContainerFixtureCollection : ICollectionFixture<ContainerFixture>
{
    // This class has no code, and is never created.
    // Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}
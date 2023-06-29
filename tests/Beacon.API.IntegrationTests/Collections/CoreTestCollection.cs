namespace Beacon.API.IntegrationTests.Collections;

[CollectionDefinition(nameof(CoreTestCollection))]
public class CoreTestCollection : ICollectionFixture<TestFixture> { }

[Collection(nameof(CoreTestCollection))]
public abstract class CoreTestBase : TestBase
{
    protected CoreTestBase(TestFixture fixture) : base(fixture)
    {
    }
}

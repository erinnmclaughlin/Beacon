using Beacon.API.IntegrationTests;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(ContainerFixture))]
namespace Beacon.API.IntegrationTests;

public sealed class ContainerFixture : IAsyncLifetime
{
    private MsSqlContainer Container { get; } = new MsSqlBuilder().Build();
    private static string DatabaseName => $"Beacon_{TestContext.Current.TestClass?.TestClassSimpleName ?? "Container"}";

    public string GetConnectionString() => Container.GetConnectionString().Replace("master", DatabaseName);

    public async ValueTask InitializeAsync()
    {
        await Container.StartAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}
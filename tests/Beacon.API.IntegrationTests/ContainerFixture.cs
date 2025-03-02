using Beacon.API.IntegrationTests;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(ContainerFixture))]
namespace Beacon.API.IntegrationTests;

public sealed class ContainerFixture : IAsyncLifetime
{
    private MsSqlContainer Container { get; } = new MsSqlBuilder().Build();

    public string GetConnectionString(string database) => Container.GetConnectionString().Replace("master", database);

    public async ValueTask InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}
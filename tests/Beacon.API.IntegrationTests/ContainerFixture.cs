using Beacon.API.IntegrationTests;
using DotNet.Testcontainers.Containers;
using Testcontainers.MsSql;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(ContainerFixture))]
namespace Beacon.API.IntegrationTests;

public sealed class ContainerFixture : IAsyncLifetime
{
    public static string StorageProvider { get; } = Environment.GetEnvironmentVariable("BEACON_STORAGE_PROVIDER") ?? WebHost.StorageProviders.MsSqlServer;

    private IDatabaseContainer Container { get; }
    private static string DatabaseName => $"Beacon_{TestContext.Current.TestClass?.TestClassSimpleName ?? "Container"}";

    public string GetConnectionString() => Container.GetConnectionString()
        .Replace("master", DatabaseName)
        .Replace("Database=postgres", $"Database={DatabaseName}");

    public ContainerFixture()
    {
        TestContext.Current.SendDiagnosticMessage("Starting {0} container...", StorageProvider);

        if (StorageProvider == WebHost.StorageProviders.Postgres)
        {
            Container = new PostgreSqlBuilder().Build();
        }
        else
        {
            Container = new MsSqlBuilder().Build();
        }
    }

    public async ValueTask InitializeAsync()
    {
        await Container.StartAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}

using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture(ContainerFixture container) : WebApplicationFactory<Program>, IAsyncLifetime
{
    public ContainerFixture Container { get; } = container;
    
    private Respawner? Checkpoint { get; set; }

    private string? _databaseName;
    public string DatabaseName => _databaseName ??= $"Beacon_{Guid.NewGuid()}";
    public string ConnectionString => Container.GetConnectionString(DatabaseName);

    /// <summary>
    /// When <see langword="true"/>, the database will be reset to the most recent checkpoint.
    /// </summary>
    public bool ShouldResetDatabase { get; set; } = true;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureServices(services =>
    {
        services.ReplaceWithTestDatabase(ConnectionString);
        services.UseFakeEmailService();
    });

    public async ValueTask InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        await dbContext.Database.MigrateAsync();
        
        Checkpoint = await Respawner.CreateAsync(ConnectionString, new RespawnerOptions
        {
            TablesToIgnore = [
                "_EFMigrationsHistory"
            ],
            WithReseed = true
        });
    }
    
    public override async ValueTask DisposeAsync()
    {
        ShouldResetDatabase = true;
        await base.DisposeAsync();
    }
    
    /// <summary>
    /// Clears all data from the database.
    /// </summary>
    public async Task ResetDatabase()
    {
        await Checkpoint!.ResetAsync(ConnectionString);
    }

    public BeaconDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BeaconDbContext>().UseSqlServer(ConnectionString).Options;
        return new BeaconDbContext(options, null!);
    }
}

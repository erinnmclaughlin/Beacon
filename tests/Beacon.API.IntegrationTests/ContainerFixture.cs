using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;

namespace Beacon.API.IntegrationTests;

public sealed class ContainerFixture : IAsyncLifetime
{
    private BeaconDbContext? _dbContext;
    private readonly Dictionary<string, object?> _metaData = [];
    
    private MsSqlContainer Container { get; } = new MsSqlBuilder().Build();
    private Respawner? Checkpoint { get; set; }

    public BeaconDbContext DbContext => _dbContext ??= CreateDbContext();
    
    public object? this[string key]
    {
        get => _metaData.GetValueOrDefault(key);
        set => _metaData[key] = value;
    }

    public string GetConnectionString() => Container.GetConnectionString().Replace("master", "Beacon");

    public async ValueTask InitializeAsync()
    {
        await Container.StartAsync();
        
        await using var dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
        
        Checkpoint = await Respawner.CreateAsync(GetConnectionString(), new RespawnerOptions
        {
            TablesToIgnore = [
                "_EFMigrationsHistory"
            ],
            WithReseed = true
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
            _dbContext = null;
        }
        
        await Container.DisposeAsync();
    }
    
    /// <summary>
    /// Clears all data from the database.
    /// </summary>
    public async Task ResetDatabase()
    {
        await Checkpoint!.ResetAsync(GetConnectionString());
    }
    
    private BeaconDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BeaconDbContext>()
            .UseSqlServer(GetConnectionString())
            .Options;
        
        return new BeaconDbContext(options, null!);
    }
}
using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.MsSql;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();
    private readonly Dictionary<string, object?> _metaData = [];
    
    private Respawner? Checkpoint { get; set; }
    
    private string? _connectionString;
    private string ConnectionString => _connectionString ??= _container.GetConnectionString().Replace("master", $"Beacon-{Guid.NewGuid()}");

    public object? this[string key]
    {
        get => _metaData.GetValueOrDefault(key);
        set => _metaData[key] = value;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase(ConnectionString);
            //services.UseMockedCurrentUser();
            //services.UseMockedLabContext();
            services.UseFakeEmailService();
        });
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

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
    
    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        
        await _container.DisposeAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Clears all data from the database and reseeds with the provided <paramref name="seedData"/>.
    /// </summary>
    /// <remarks>
    /// Note that the database is ALWAYS seeded with <see cref="TestData.Lab"/>.
    /// </remarks>
    /// <param name="seedData">The seed data to reseed the database with.</param>
    public async Task ResetDatabase(params object[] seedData)
    {
        await Checkpoint!.ResetAsync(ConnectionString);
        
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        dbContext.Add(TestData.Lab);
        dbContext.AddRange(seedData);
        await dbContext.SaveChangesAsync();
    }
}

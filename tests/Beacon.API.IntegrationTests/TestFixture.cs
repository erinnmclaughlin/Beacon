using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture(ContainerFixture container) : WebApplicationFactory<Program>, IAsyncLifetime
{
    private BeaconDbContext? _dbContext;
    
    public string ConnectionString => container.GetConnectionString();
    public BeaconDbContext DbContext => _dbContext ??= CreateDbContext();
    
    public bool IsSeeded { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureServices(services =>
    {
        services.ReplaceWithTestDatabase(ConnectionString);
        services.UseFakeEmailService();
    });

    public async ValueTask InitializeAsync()
    {
        await DbContext.Database.MigrateAsync();
    }
    
    public override async ValueTask DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
        
        await base.DisposeAsync();
    }

    public async Task ApplySeedData(object[] seedData)
    {
        DbContext.AddRange(seedData);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        IsSeeded = true;
        DbContext.ChangeTracker.Clear();
    }

    private BeaconDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BeaconDbContext>().UseSqlServer(ConnectionString).Options;
        return new BeaconDbContext(options, null!);
    }
}

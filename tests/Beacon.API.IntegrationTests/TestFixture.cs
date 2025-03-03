using Beacon.API.Persistence;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture(ContainerFixture container) : WebApplicationFactory<Program>, IAsyncLifetime
{
    public string ConnectionString => container.GetConnectionString();
    
    public bool IsSeeded { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureServices(services =>
    {
        services.ReplaceWithTestDatabase(ConnectionString);
        services.UseFakeEmailService();
    });

    public async ValueTask InitializeAsync()
    {
        await using var dbContext = CreateDbContext(null!);
        await dbContext.Database.MigrateAsync();
    }
    
    public override async ValueTask DisposeAsync()
    {
        await using var dbContext = CreateDbContext(null!);
        await dbContext.Database.EnsureDeletedAsync();
        
        await base.DisposeAsync();
    }

    public async Task ApplySeedData(object[] seedData)
    {
        await using var dbContext = CreateDbContext(null!);
        dbContext.AddRange(seedData);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        IsSeeded = true;
    }

    public BeaconDbContext CreateDbContext(ISessionContext context)
    {
        var options = new DbContextOptionsBuilder<BeaconDbContext>().UseSqlServer(ConnectionString).Options;
        return new BeaconDbContext(options, context);
    }
}

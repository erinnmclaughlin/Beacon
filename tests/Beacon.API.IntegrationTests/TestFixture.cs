using Beacon.API.Persistence;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture(ContainerFixture container) : WebApplicationFactory<Program>, IAsyncLifetime
{
    public string ConnectionString { get; } = container.GetConnectionString();

    public bool IsSeeded { get; private set; }

    public static string StorageProvider => ContainerFixture.StorageProvider;

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
        var optionsBuilder = new DbContextOptionsBuilder<BeaconDbContext>();
        BeaconWebHost.ConfigureDbContextOptions(StorageProvider, ConnectionString)(optionsBuilder);

        return new BeaconDbContext(optionsBuilder.Options, context);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureServices((context, services) =>
        {
            context.Configuration["StorageProvider"] = StorageProvider;

            services.ReplaceWithTestDatabase(context, ConnectionString);
            services.UseFakeEmailService();
        });
    }
}

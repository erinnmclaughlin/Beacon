using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;

namespace Beacon.API.IntegrationTests;

public class SetupFixture : IAsyncLifetime
{
    private readonly IConfigurationRoot _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    private Guid? _currentUserId;
    private Respawner _respawner = null!;

    public SetupFixture()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        BeaconWebHost.ConfigureServices(services, _configuration);

        _scopeFactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync("SqlServerDb");
        _currentUserId = null;
    }

    public async Task InitializeAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        await dbContext.Database.MigrateAsync();
        dbContext.Users.AddRange(
            TestData.AdminUser,
            TestData.AdminUserAlt,
            TestData.ManagerUser,
            TestData.ManagerUserAlt,
            TestData.AnalystUser,
            TestData.AnalystUserAlt,
            TestData.MemberUser,
            TestData.MemberUserAlt,
            TestData.NonMemberUser);
        dbContext.Laboratories.Add(TestData.Lab);
        await dbContext.SaveChangesAsync();

        _respawner = await Respawner.CreateAsync(dbContext.Database.GetDbConnection(), new RespawnerOptions
        {
            TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
        });
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

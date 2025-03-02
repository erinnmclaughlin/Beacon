using Microsoft.AspNetCore.Hosting;

namespace Beacon.API.IntegrationTests;

public sealed class TestFixture(ContainerFixture container) : WebApplicationFactory<Program>, IAsyncLifetime
{
    public ContainerFixture Container { get; } = container;
    
    /// <summary>
    /// When <see langword="true"/>, the database will be reset to the most recent checkpoint.
    /// </summary>
    public bool ShouldResetDatabase
    {
        get => Container[nameof(ShouldResetDatabase)] is null or true;
        set => Container[nameof(ShouldResetDatabase)] = value;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.ReplaceWithTestDatabase(Container.GetConnectionString());
            services.UseFakeEmailService();
        });
    }

    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }
    
    public override async ValueTask DisposeAsync()
    {
        ShouldResetDatabase = true;
        await base.DisposeAsync();
    }
}

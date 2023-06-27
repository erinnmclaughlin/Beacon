using Beacon.API.Persistence;
using Beacon.WebHost;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var builder = WebApplication.CreateBuilder(args);

BeaconWebHost.ConfigureServices(builder.Services, builder.Configuration);

if (builder.Environment.IsEnvironment("Test"))
{
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
}

var app = BeaconWebHost.Configure(builder.Build());

if (builder.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var testDb = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
    await testDb.InitializeForTests();
}

app.Run();

using Beacon.API;
using Beacon.API.Persistence;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBeaconApi(builder.Configuration, options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServerDb");
    options.UseSqlServer(connectionString);
});

if (builder.Environment.IsEnvironment("Test"))
{
    StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
}

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapBeaconEndpoints();
app.MapGet("api/ping", () => Results.Ok("pong"));

app.MapFallbackToFile("index.html");

if (builder.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var testDb = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
    await testDb.Database.EnsureDeletedAsync();
    await testDb.Database.MigrateAsync();
}

app.Run();

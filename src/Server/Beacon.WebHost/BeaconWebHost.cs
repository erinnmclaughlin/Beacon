using Beacon.API;
using Beacon.API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Beacon.WebHost;

public static class BeaconWebHost
{
    public static WebApplication BuildBeaconApplication(this WebApplicationBuilder builder)
    {
        var storageProvider = builder.Configuration.GetValue<string>("StorageProvider") ?? StorageProviders.MsSqlServer;
        var connectionString = builder.Configuration.GetConnectionString(storageProvider);

        builder.Services.AddBeaconApi(builder.Configuration, ConfigureDbContextOptions(storageProvider, connectionString));

        builder.Services.AddSwaggerGen();

        return builder.Build();
    }

    public static Action<DbContextOptionsBuilder> ConfigureDbContextOptions(string storageProvider, string? connectionString)
    {
        if (connectionString is null)
            return o => { };

        if (storageProvider.Equals(StorageProviders.Postgres, StringComparison.OrdinalIgnoreCase))
            return BeaconPostgresStorageProvider.BuildDbContextOptionsBuilder(connectionString);

        if (storageProvider.Equals(StorageProviders.MsSqlServer, StringComparison.OrdinalIgnoreCase))
            return BeaconMsSqlStorageProvider.ConfigureDbContextOptions(connectionString);

        throw new NotSupportedException($"Storage provider '{storageProvider}' is not supported.");
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
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
        app.MapGet("api/ping", () => Results.Ok("pong")).ExcludeFromDescription();
        app.Map("api/{**slug}", () => Results.NotFound("Unrecognized endpoint.")).ExcludeFromDescription();

        app.MapFallbackToFile("index.html");

        return app;
    }
}

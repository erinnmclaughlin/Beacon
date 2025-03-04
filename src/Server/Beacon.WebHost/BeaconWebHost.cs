using Beacon.API;
using Beacon.API.Extensions;

namespace Beacon.WebHost;

public static class BeaconWebHost
{
    public static WebApplication BuildBeaconApplication(this WebApplicationBuilder builder)
    {
        var storageProvider = builder.Configuration.GetValue<string>("StorageProvider") ?? "MsSqlServer";
        var connectionString = builder.Configuration.GetConnectionString(storageProvider);

        builder.Services.AddBeaconApi(builder.Configuration, storageProvider switch
        {
            "MsSqlServer" => BeaconMsSqlStorageProvider.ConfigureDbContextOptions(connectionString),
            "Postgres" => BeaconPostgresStorageProvider.BuildDbContextOptionsBuilder(connectionString),
            _ => throw new NotSupportedException($"Storage provider '{storageProvider}' is not supported.")
        });

        builder.Services.AddSwaggerGen();

        return builder.Build();
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

using Beacon.API;
using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Beacon.WebHost;

public class BeaconWebHost 
{ 
    public static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddBeaconApi(config, options =>
        {
            var connectionString = config.GetConnectionString("SqlServerDb");
            options.UseSqlServer(connectionString);
        });

        services.AddSwaggerGen();
    }

    public static WebApplication Configure(WebApplication app)
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

        app.MapFallbackToFile("index.html");

        return app;
    }
}

using Beacon.API;
using Microsoft.EntityFrameworkCore;

namespace Beacon.WebHost;

public static class BeaconWebHost
{
    public static WebApplication BuildBeaconApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddBeaconApi(builder.Configuration, o =>
        {
            o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDb"));
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

        app.UseBeacon();
        app.MapGet("api/ping", () => Results.Ok("pong")).ExcludeFromDescription();

        app.MapFallbackToFile("index.html");

        return app;
    }
}

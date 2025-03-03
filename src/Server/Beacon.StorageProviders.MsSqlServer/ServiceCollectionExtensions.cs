using Beacon.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Beacon;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddBeaconApi(this WebApplicationBuilder builder, string connectionStringName = "BeaconDb")
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{connectionStringName}' was not found in configuration.");
        }

        builder.Services.AddBeaconApi(builder.Configuration, o =>
        {
            o.UseSqlServer(connectionString, b =>
            {
                b.MigrationsAssembly(typeof(ServiceCollectionExtensions).Assembly);
            });
        });

        return builder;
    }
}

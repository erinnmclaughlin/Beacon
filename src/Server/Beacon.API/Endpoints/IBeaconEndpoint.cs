using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace Beacon.API.Endpoints;

public interface IBeaconEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}

internal static class EndpointMapper
{
    public static void MapBeaconEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i == typeof(IBeaconEndpoint)));

        foreach (var endpoint in endpoints)
        {
            var mapMethod = endpoint.GetMethod(nameof(IBeaconEndpoint.Map), types: new[] { typeof(IEndpointRouteBuilder) });
            mapMethod!.Invoke(null, new object[] { app });
        }
    }
}
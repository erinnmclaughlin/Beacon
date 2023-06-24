using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Auth;

public sealed class Logout : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("auth/logout", (HttpContext context) => context.SignOutAsync()).WithTags(EndpointTags.Authentication);
    }
}

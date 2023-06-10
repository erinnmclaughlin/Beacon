using Microsoft.AspNetCore.Routing;

namespace Beacon.API;

public interface IApiEndpointMapper
{
    static abstract void Map(IEndpointRouteBuilder app);
}

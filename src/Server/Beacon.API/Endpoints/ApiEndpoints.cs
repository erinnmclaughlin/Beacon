using Beacon.App.Features.Laboratories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints;

public sealed class ApiEndpoints : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var baseApi = app.MapGroup("api").RequireAuthorization();

        baseApi.MapPost("lab", async (Guid labId, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new SetCurrentLaboratory.Command(labId), ct);
            return Results.NoContent();
        });

        var api = baseApi.MapGroup("").RequireAuthorization("ApiAuth");

        api.MapGet("lab", async (ISender sender, CancellationToken ct) =>
        {
            var response = await sender.Send(new GetCurrentLaboratory.Query(), ct);
            return response.Laboratory is { } lab ? Results.Ok(lab) : Results.NoContent();
        });

        InvitationsEndpoints.Map(api);
        MembershipsEndpoint.Map(api);
    }
}

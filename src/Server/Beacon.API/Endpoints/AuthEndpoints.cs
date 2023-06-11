using Beacon.App.Features.Auth;
using Beacon.Common.Auth.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints;

internal sealed class AuthEndpoints : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var authGroup = app.MapGroup("auth");

        authGroup
            .MapGet("me", async (ISender sender, CancellationToken ct) =>
            {
                var currentUser = await sender.Send(new GetCurrentUser.Query(), ct);
                return Results.Ok(currentUser);
            });

        authGroup
            .MapPost("login", async (LoginRequest request, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new Login.Command(request.EmailAddress, request.Password), ct);
                return Results.NoContent();
            })
            .AllowAnonymous();

        authGroup
            .MapPost("register", async (RegisterRequest request, ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new Register.Command
                {
                    DisplayName = request.DisplayName,
                    EmailAddress = request.EmailAddress,
                    PlainTextPassword = request.Password
                }, ct);

                await sender.Send(new Login.Command(request.EmailAddress, request.Password), ct);

                return Results.NoContent();
            })
            .AllowAnonymous();

        authGroup
            .MapGet("logout", async (ISender sender, CancellationToken ct) =>
            {
                await sender.Send(new Logout.Command(), ct);
                return Results.NoContent();
            })
            .AllowAnonymous();
    }
}

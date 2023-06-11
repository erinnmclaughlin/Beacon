using Beacon.App.Features.Auth;
using Beacon.App.Features.Laboratories;
using Beacon.Common.Auth.Requests;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints;

public sealed class PortalEndpoints : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var portal = app.MapGroup("portal");

        portal.MapGet("me", async (ISender sender, CancellationToken ct) =>
        {
            var currentUser = await sender.Send(new GetCurrentUser.Query(), ct);
            return Results.Ok(currentUser);
        }).RequireAuthorization();

        portal.MapPost("login", async (LoginRequest request, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new Login.Command(request.EmailAddress, request.Password), ct);
            return Results.NoContent();
        });

        portal.MapPost("register", async (RegisterRequest request, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new Register.Command
            {
                DisplayName = request.DisplayName,
                EmailAddress = request.EmailAddress,
                PlainTextPassword = request.Password
            }, ct);

            await sender.Send(new Login.Command(request.EmailAddress, request.Password), ct);

            return Results.NoContent();
        });

        portal.MapGet("logout", async (ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new Logout.Command(), ct);
            return Results.NoContent();
        });

        portal.MapPost("laboratories", async (CreateLaboratoryRequest request, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new CreateLaboratory.Command(request.LaboratoryName), ct);
            return Results.NoContent();
        }).RequireAuthorization();

        portal.MapGet("invitations/{inviteId:Guid}/accept", async (Guid inviteId, Guid emailId, ISender sender, CancellationToken ct) =>
        {
            var command = new AcceptEmailInvitation.Command(inviteId, emailId);
            await sender.Send(command, ct);
            return Results.NoContent();
        });
    }
}

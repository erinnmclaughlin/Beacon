using Beacon.App.Features.Laboratories;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints;

internal sealed class InvitationsEndpoints : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("invitations");

        group.MapPost("", async (InviteLabMemberRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new InviteNewMember.Command
            {
                NewMemberEmailAddress = request.NewMemberEmailAddress,
                MembershipType = request.MembershipType
            };

            await sender.Send(command, ct);
            return Results.NoContent();
        });
    }
}

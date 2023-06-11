using Beacon.App.Features.Laboratories;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints;

internal sealed class MembershipsEndpoint : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("memberships");

        group.MapPut("{memberId:Guid}/membershipType", async (Guid memberId, UpdateMembershipTypeRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateUserMembership.Command(memberId, request.MembershipType);
            await sender.Send(command, ct);
            return Results.NoContent();
        });
    }
}

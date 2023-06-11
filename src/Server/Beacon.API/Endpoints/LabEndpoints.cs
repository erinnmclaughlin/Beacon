using Beacon.App.Features.Laboratories;
using Beacon.App.Helpers;
using Beacon.App.Services;
using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Beacon.API.Endpoints;

internal sealed class LabEndpoints : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("laboratories/{labId:Guid}/sign-in", SignIn);
        app.MapGet("laboratories/current", GetCurrentLabDetails).RequireAuthorization("LabMember");
        app.MapPost("laboratories", Create);
        app.MapGet("laboratories/{labId:Guid}", GetDetails);
        app.MapPost("laboratories/{labId:Guid}/invitations", InviteMember);
        app.MapPut("laboratories/{labId}/memberships/{memberId}/membershipType", UpdateMembershipType);
        app.MapGet("users/me/memberships", GetCurrentUserMemberships);
        app.MapGet("users/{memberId:Guid}/memberships", GetMembershipsByMemberId);
        app.MapGet("invitations/{inviteId:Guid}/accept", AcceptInvitation);
    }

    private static async Task<IResult> SignIn(Guid labId, ICurrentUser currentUser, HttpContext context, ISender sender, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var getLabResponse = await sender.Send(new GetLaboratoryDetails.Query(labId), ct);

        if (getLabResponse.Laboratory?.Members.FirstOrDefault(m => m.Id == userId) is not { } member)
            return Results.NotFound();

        var membershipIdentity = new ClaimsIdentity("LabAuth");
        membershipIdentity.AddClaim(new Claim("LaboratoryId", labId.ToString()));
        membershipIdentity.AddClaim(new Claim(ClaimTypes.Role, member.MembershipType.ToString()));

        context.User.AddIdentity(membershipIdentity);

        await context.SignInAsync(context.User);
        return Results.NoContent();
    }

    private static async Task<IResult> GetCurrentLabDetails(HttpContext context, ISender sender, CancellationToken ct)
    {
        var labIdentity = context.User.Identities.First(i => i.AuthenticationType == "LabAuth");
        var labIdValue = labIdentity.FindFirst("LaboratoryId")?.Value;

        if (!Guid.TryParse(labIdValue ?? "", out var labId))
            return Results.NotFound();

        var result = await sender.Send(new GetLaboratoryDetails.Query(labId), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(CreateLaboratoryRequest request, ISender sender, CancellationToken ct)
    {
        var command = new CreateLaboratory.Command
        {
            LaboratoryName = request.LaboratoryName
        };

        await sender.Send(command, ct);

        return Results.Created($"laboratories/{command.LaboratoryId}", new LaboratoryDto
        {
            Id = command.LaboratoryId,
            Name = request.LaboratoryName
        });
    }

    public static async Task<IResult> GetDetails(Guid labId, ISender sender, CancellationToken ct)
    {
        var query = new GetLaboratoryDetails.Query(labId);
        var response = await sender.Send(query, ct);

        return response.Laboratory is not { } lab ? Results.NotFound() : Results.Ok(new LaboratoryDetailDto
        {
            Id = lab.Id,
            Name = lab.Name,
            Members = lab.Members.Select(m => new LaboratoryMemberDto
            {
                Id = m.Id,
                DisplayName = m.DisplayName,
                EmailAddress = m.EmailAddress,
                MembershipType = m.MembershipType
            }).ToList()
        });
    }

    private static async Task<IResult> GetCurrentUserMemberships(ClaimsPrincipal user, ISender sender, CancellationToken ct)
    {
        var currentUserId = user.GetUserId();
        return await GetMembershipsByMemberId(currentUserId, sender, ct);
    }

    private static async Task<IResult> GetMembershipsByMemberId(Guid memberId, ISender sender, CancellationToken ct)
    {
        var query = new GetUserMemberships.Query(memberId);
        var result = await sender.Send(query, ct);

        var memberships = result.Memberships
            .Select(m => new LaboratoryMembershipDto
            {
                Laboratory = new LaboratoryDto
                {
                    Id = m.Laboratory.Id,
                    Name = m.Laboratory.Name
                },
                Member = new UserDto
                {
                    Id = m.Member.Id,
                    DisplayName = m.Member.DisplayName,
                    EmailAddress = m.Member.EmailAddress
                },
                MembershipType = m.MembershipType
            })
            .ToList();

        return Results.Ok(memberships);
    }

    private static async Task<IResult> InviteMember(Guid labId, InviteLabMemberRequest request, ISender sender, CancellationToken ct)
    {
        var command = new InviteNewMember.Command
        {
            NewMemberEmailAddress = request.NewMemberEmailAddress,
            MembershipType = request.MembershipType,
            LaboratoryId = labId
        };

        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> AcceptInvitation(Guid inviteId, Guid emailId, ISender sender, CancellationToken ct)
    {
        var command = new AcceptEmailInvitation.Command(inviteId, emailId);
        await sender.Send(command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateMembershipType(Guid labId, Guid memberId, UpdateMembershipTypeRequest request, ISender sender, CancellationToken ct)
    {
        var command = new UpdateUserMembership.Command(labId, memberId, request.MembershipType);
        await sender.Send(command, ct);
        return Results.NoContent();
    }
}

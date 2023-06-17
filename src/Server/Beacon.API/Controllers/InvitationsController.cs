using Beacon.App.Features.Invitations;
using Beacon.Common.Laboratories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/invitations")]
public sealed class InvitationsController : ApiControllerBase
{
    [Authorize(AuthConstants.LabAuth), HttpPost]
    public async Task<IActionResult> SendEmailInvitation(InviteLabMemberRequest request, CancellationToken ct)
    {
        await ExecuteAsync(new InviteMember.Command
        {
            NewMemberEmailAddress = request.NewMemberEmailAddress,
            MembershipType = request.MembershipType
        }, ct);

        return NoContent();
    }

    [HttpGet("{inviteId:Guid}/accept")]
    public async Task<IActionResult> AcceptInvite(Guid inviteId, Guid emailId, CancellationToken ct)
    {
        await ExecuteAsync(new AcceptEmailInvitation.Command(inviteId, emailId), ct);
        return NoContent();
    }
}

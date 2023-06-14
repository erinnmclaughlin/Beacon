using Beacon.App.Features.Laboratories.Commands;
using Beacon.Common.Laboratories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Authorize(AuthConstants.LabAuth), Route("api/members")]
public sealed class MembersController : ApiControllerBase
{
    [HttpPut("{memberId:Guid}/membershipType")]
    public async Task<IActionResult> UpdateMembershipType(Guid memberId, UpdateMembershipTypeRequest request, CancellationToken ct)
    {
        await ExecuteAsync(new UpdateUserMembership.Command(memberId, request.MembershipType), ct);
        return NoContent();
    }
}

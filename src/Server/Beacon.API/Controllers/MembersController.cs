using Beacon.App.Features.Memberships;
using Beacon.Common.Memberships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Authorize(AuthConstants.LabAuth), Route("api/members")]
public sealed class MembersController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMembers(CancellationToken ct)
    {
        var members = await GetAsync(new GetMembers.Query(), ct);
        return Ok(members);
    }

    [HttpPut("{memberId:Guid}/membershipType")]
    public async Task<IActionResult> UpdateMembershipType(Guid memberId, UpdateMembershipTypeRequest request, CancellationToken ct)
    {
        await ExecuteAsync(new UpdateMembershipType.Command(memberId, request.MembershipType), ct);
        return NoContent();
    }
}

using Beacon.App.Features.Laboratories.Commands;
using Beacon.App.Features.Laboratories.Queries;
using Beacon.Common.Laboratories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/laboratories")]
public sealed class LaboratoriesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLaboratories(CancellationToken ct)
    {
        var labs = await GetAsync(new GetMyLaboratoriesFeature.Query(), ct);
        return Ok(labs);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLaboratory(CreateLaboratoryRequest request, CancellationToken ct)
    {
        await ExecuteAsync(new CreateLaboratory.Command(request.LaboratoryName), ct);
        return NoContent();
    }

    [HttpPost("{labId:Guid}/login")]
    public async Task<IActionResult> LoginToLaboratory(Guid labId, CancellationToken ct)
    {
        await ExecuteAsync(new LoginToLaboratory.Command(labId), ct);
        return NoContent();
    }

    [Authorize(AuthConstants.LabAuth), HttpGet("logout")]
    public async Task<IActionResult> LogoutOfLaboratory(CancellationToken ct)
    {
        await ExecuteAsync(new LogoutOfLaboratory.Command(), ct);
        return NoContent();
    }
}

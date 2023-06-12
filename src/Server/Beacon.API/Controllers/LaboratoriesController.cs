using Beacon.App.Features.Laboratories;
using Beacon.Common.Laboratories.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/laboratories")]
public sealed class LaboratoriesController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateLaboratory(CreateLaboratoryRequest request, CancellationToken ct)
    {
        await ExecuteAsync(new CreateLaboratory.Command(request.LaboratoryName), ct);
        return NoContent();
    }

    [HttpPost("{labId:Guid}/login")]
    public async Task<IActionResult> LoginToLaboratory(Guid labId, CancellationToken ct)
    {
        await ExecuteAsync(new SetCurrentLaboratory.Command(labId), ct);
        return NoContent();
    }

    [Authorize(AuthConstants.LabAuth), HttpGet("current")]
    public async Task<IActionResult> GetCurrentLaboratory(CancellationToken ct)
    {
        var response = await GetAsync(new GetCurrentLaboratory.Query(), ct);
        return response.Laboratory is { } lab ? Ok(lab) : NotFound();
    }
}

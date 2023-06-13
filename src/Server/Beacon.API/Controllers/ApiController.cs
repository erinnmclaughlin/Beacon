using Beacon.App.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api")]
public sealed class ApiController : ApiControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        return Ok(await GetAsync(new GetCurrentUser.Query(), ct));
    }
}

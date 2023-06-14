using Beacon.App.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/session")]
public sealed class SessionController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        return Ok(await GetAsync(new GetSessionInfo.Query(), ct));
    }
}

using Beacon.Common.Auth.GetCurrentUser;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await Mediator.Send(new GetCurrentUserRequest());
        return result.IsError ? NotFound() : Ok(result.Value);
    }
}

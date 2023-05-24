using Beacon.Common.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    [HttpGet("current")]
    public IActionResult GetCurrentUser()
    {
        var user = HttpContext.User.ToUserDto();

        return user is null ? NotFound() : Ok(user);
    }
}

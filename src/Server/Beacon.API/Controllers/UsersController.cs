using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Beacon.API.Controllers;

[ApiController, Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("current")]
    public Task<string?> GetCurrentUser()
    {
        return Task.FromResult(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}

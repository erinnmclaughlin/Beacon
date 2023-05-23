using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/[controller]")]
public sealed class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = await Mediator.Send(request);

        await HttpContext.SignInAsync(user.ToClaimsPrincipal());

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await Mediator.Send(request);

        await HttpContext.SignInAsync(user.ToClaimsPrincipal());

        return Ok(user);
    }

    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
    }
}

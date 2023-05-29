using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Route("api/[controller]")]
public sealed class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await Mediator.Send(request);
        return await GetLoginResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await Mediator.Send(request);
        return await GetLoginResult(result);
    }

    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
    }

    private async Task<IActionResult> GetLoginResult(ErrorOr<UserDto> result)
    {
        if (result.IsError)
            return ValidationProblem(result.Errors);

        var user = result.Value;
        await HttpContext.SignInAsync(user.ToClaimsPrincipal());

        return Ok(user);
    }
}

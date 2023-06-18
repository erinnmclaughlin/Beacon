using Beacon.App.Features.Auth;
using Beacon.Common.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[AllowAnonymous, Route("api/auth")]
public sealed class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        await ExecuteAsync(new Login.Command(request.EmailAddress, request.Password), ct);
        return NoContent();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        await ExecuteAsync(new Register.Command
        {
            DisplayName = request.DisplayName,
            EmailAddress = request.EmailAddress,
            PlainTextPassword = request.Password
        }, ct);

        await ExecuteAsync(new Login.Command(request.EmailAddress, request.Password), ct);

        return NoContent();
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await ExecuteAsync(new Logout.Command(), ct);
        return NoContent();
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Beacon.API.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task Login(LoginRequest request)
    {
        if (request.Password != $"!!{request.Username}")
            return;
        
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.Username));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }

    [HttpPost("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
    }
}

public record LoginRequest(string Username, string Password);

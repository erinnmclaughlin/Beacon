using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Beacon.API.Controllers;

[ApiController, Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly BeaconDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AuthController(BeaconDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.EmailAddress == request.EmailAddress))
        {
            ModelState.AddModelError(nameof(RegisterRequest.EmailAddress), "An account with the specified email address already exists.");
            return ValidationProblem();
        }

        var hashedPassword = _passwordHasher.Hash(request.Password, out var salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            EmailAddress = request.EmailAddress,
            HashedPassword = hashedPassword,
            HashedPasswordSalt = salt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);

        if (user is null || !_passwordHasher.Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt))
        {
            return ValidationProblem("Email address or password was incorrect.");
        }

        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Email, user.EmailAddress));

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return Ok(new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName
        });
    }

    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
    }
}

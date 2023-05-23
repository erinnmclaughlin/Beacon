using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using Beacon.Common.Auth.Register;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        var email = request.EmailAddress.Trim();

        if (await _context.Users.AnyAsync(u => u.EmailAddress == email))
        {
            ModelState.AddModelError(nameof(RegisterRequest.EmailAddress), "An account with the specified email address already exists.");
            return ValidationProblem();
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName.Trim(),
            EmailAddress = email,
            HashedPassword = _passwordHasher.Hash(request.Password, out var salt),
            HashedPasswordSalt = salt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        };

        await HttpContext.SignInAsync(userDto.ToClaimsPrincipal());

        return Ok(userDto);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.EmailAddress.Trim();

        var user = await _context.Users
            .Where(u => u.EmailAddress == email)
            .Select(u => new
            {
                Response = new UserDto
                {
                    Id = u.Id,
                    EmailAddress = u.EmailAddress,
                    DisplayName = u.DisplayName,
                },
                u.HashedPassword,
                u.HashedPasswordSalt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (user is null || !_passwordHasher.Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt))
        {
            ModelState.AddModelError(nameof(LoginRequest.EmailAddress), "Email address or password was incorrect.");
            return ValidationProblem();
        }

        await HttpContext.SignInAsync(user.Response.ToClaimsPrincipal());

        return Ok(user.Response);
    }

    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
    }
}

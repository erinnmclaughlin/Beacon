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

        var hashedPassword = _passwordHasher.Hash(request.Password, out var salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName.Trim(),
            EmailAddress = email,
            HashedPassword = hashedPassword,
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
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt))
        {
            ModelState.AddModelError(nameof(LoginRequest.EmailAddress), "Email address or password was incorrect.");
            return ValidationProblem();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        };

        await HttpContext.SignInAsync(userDto.ToClaimsPrincipal());

        return Ok(userDto);
    }

    [HttpGet("logout")]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync();
    }
}

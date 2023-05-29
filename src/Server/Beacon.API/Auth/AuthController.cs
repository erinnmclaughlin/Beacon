using Beacon.API.Helpers;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Auth;

[ApiController, Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _mediator.Send(new GetCurrentUserRequest());
        return result.IsError ? NotFound() : Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _mediator.Send(request);
        return GetLoginResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _mediator.Send(request);
        return GetLoginResult(result);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _mediator.Send(new LogoutRequest());
        return result.IsError ? StatusCode(500) : NoContent();
    }

    private IActionResult GetLoginResult(ErrorOr<UserDto> result)
    {
        return result.IsError ? result.Errors.ToValidationProblemResult() : Ok(result.Value);
    }
}

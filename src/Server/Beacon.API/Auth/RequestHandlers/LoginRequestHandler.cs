using Beacon.API.Auth.Services;
using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Auth.RequestHandlers;

internal class LoginRequestHandler : IApiRequestHandler<LoginRequest, UserDto>
{
    private readonly BeaconDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISignInManager _signInManager;

    public LoginRequestHandler(BeaconDbContext context, IPasswordHasher passwordHasher, ISignInManager signInManager)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _signInManager = signInManager;
    }

    public async Task<ErrorOr<UserDto>> Handle(LoginRequest request, CancellationToken ct)
    {
        var user = await _context.Users
            .Where(u => u.EmailAddress == request.EmailAddress)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (user is null || !_passwordHasher.Verify(request.Password, user.HashedPassword, user.HashedPasswordSalt))
        {
            return Error.Validation(nameof(LoginRequest.EmailAddress), "Email address or password was incorrect.");
        }

        var result = new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        };

        await _signInManager.SignInAsync(result.ToClaimsPrincipal());

        return result;
    }
}

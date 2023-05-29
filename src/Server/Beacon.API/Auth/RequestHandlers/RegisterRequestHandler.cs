using Beacon.API.Auth.Services;
using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Auth.RequestHandlers;

internal class RegisterRequestHandler : IApiRequestHandler<RegisterRequest, UserDto>
{
    private readonly BeaconDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISignInManager _signInManager;

    public RegisterRequestHandler(BeaconDbContext context, IPasswordHasher passwordHasher, ISignInManager signInManager)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _signInManager = signInManager;
    }

    public async Task<ErrorOr<UserDto>> Handle(RegisterRequest request, CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(u => u.EmailAddress == request.EmailAddress, ct))
            return Error.Validation(nameof(RegisterRequest.EmailAddress), "An account with the specified email address already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            EmailAddress = request.EmailAddress,
            HashedPassword = _passwordHasher.Hash(request.Password, out var salt),
            HashedPasswordSalt = salt
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);

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

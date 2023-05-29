using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Auth;

public class LoginRequestHandler : IApiRequestHandler<LoginRequest, UserDto>
{
    private readonly BeaconDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public LoginRequestHandler(BeaconDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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

        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        };
    }
}

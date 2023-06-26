using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Beacon.API.Services;

public sealed class BeaconAuthenticationService
{
    private readonly BeaconDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public BeaconAuthenticationService(BeaconDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<ClaimsPrincipal> AuthenticateAsync(string email, string password, CancellationToken ct)
    {
        var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.EmailAddress == email, ct);

        if (user is null || !_passwordHasher.Verify(password, user.HashedPassword, user.HashedPasswordSalt))
            return new ClaimsPrincipal(new ClaimsIdentity()); // anonymous (unauthenticated) user

        return CreateClaimsPrincipal(user);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        var identity = new ClaimsIdentity("AuthCookie");
        identity.AddClaim(BeaconClaimTypes.UserId, user.Id.ToString());

        return new ClaimsPrincipal(identity);
    }
}

using Beacon.API.Persistence;
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

    public async Task<ClaimsIdentity> AuthenticateAsync(string email, string password, CancellationToken ct)
    {
        var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.EmailAddress == email, ct);

        if (user is null || !_passwordHasher.Verify(password, user.HashedPassword, user.HashedPasswordSalt))
            return new ClaimsIdentity(); // anonymous identity

        var identity = new ClaimsIdentity("AuthCookie");
        identity.AddClaim(BeaconClaimTypes.UserId, user.Id.ToString());
        identity.AddClaim(BeaconClaimTypes.DisplayName, user.DisplayName);
        identity.AddClaim(BeaconClaimTypes.Email, user.EmailAddress);
        return identity;
    }
}

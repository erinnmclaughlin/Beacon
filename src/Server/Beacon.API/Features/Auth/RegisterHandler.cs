using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.API.Services;
using Beacon.Common.Requests.Auth;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Auth;

internal sealed class RegisterHandler : IBeaconRequestHandler<RegisterRequest>
{
    private readonly BeaconDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterHandler(BeaconDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<Success>> Handle(RegisterRequest request, CancellationToken ct)
    {
        if (await _dbContext.Users.AnyAsync(u => u.EmailAddress == request.EmailAddress, ct))
        {
            return Error.Validation(
                nameof(RegisterRequest.EmailAddress), 
                "An account with the specified email address already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            EmailAddress = request.EmailAddress,
            HashedPassword = _passwordHasher.Hash(request.Password, out var salt),
            HashedPasswordSalt = salt
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}

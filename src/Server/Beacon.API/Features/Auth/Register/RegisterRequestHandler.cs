using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Register;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Auth.Register;

public class RegisterRequestHandler : IRequestHandler<RegisterRequest, UserDto>
{
    private readonly BeaconDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterRequestHandler(BeaconDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(RegisterRequest request, CancellationToken ct)
    {
        await EnsureEmailAddressIsUnique(request.EmailAddress, ct); 
        
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

        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        };
    }

    private async Task EnsureEmailAddressIsUnique(string email, CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(u => u.EmailAddress == email, ct) == false)
            return;

        var failure = new ValidationFailure(
            nameof(RegisterRequest.EmailAddress),
            "An account with the specified email address already exists.",
            email);

        throw new ValidationException(new List<ValidationFailure> { failure });
    }
}

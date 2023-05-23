using Beacon.API.Entities;
using Beacon.API.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Endpoints.Auth.Register;

public static class RegisterEndpoint
{
    public static async Task<Results<Ok, BadRequest>> Handle(RegisterRequest request, BeaconDbContext context, CancellationToken ct)
    {
        if (context.Users.Any(u => u.EmailAddress == request.EmailAddress))
        {
            return TypedResults.BadRequest("An account already exists with the specified email address.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = request.EmailAddress,
            HashedPassword = "TODO: Hash password!"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(user.Id);
    }
}

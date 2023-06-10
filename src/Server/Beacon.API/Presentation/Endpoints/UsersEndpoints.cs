using Beacon.API.App.Features.Users;
using Beacon.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Presentation.Endpoints;

internal class UsersEndpoints : IApiEndpointMapper
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{id:Guid}", GetById);
        app.MapGet("users/{email}", GetByEmail);
    }

    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken ct)
    {
        var query = new GetUserById.Query(id);
        var response = await sender.Send(query, ct);

        if (response.User is not { } user)
            return Results.NotFound($"User with id {id} was not found.");

        return Results.Ok(new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        });
    }

    private static async Task<IResult> GetByEmail(string email, ISender sender, CancellationToken ct)
    {
        var query = new GetUserByEmailAddress.Query(email);
        var response = await sender.Send(query, ct);

        if (response.User is not { } user)
            return Results.NotFound($"User with email addresss {email} was not found.");

        return Results.Ok(new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress
        });
    }
}

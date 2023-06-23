using Beacon.API.Services;
using Beacon.App.Exceptions;
using Beacon.Common.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Auth;

public sealed class Login : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost("auth/login", async (LoginRequest request, BeaconAuthenticationService authService, HttpContext context, CancellationToken ct) =>
        {
            var user = await authService.AuthenticateAsync(request.EmailAddress, request.Password, ct);

            if (user.Identity?.IsAuthenticated is not true)
                throw new BeaconValidationException(nameof(LoginRequest.EmailAddress), "Email address or password is invalid.");

            await context.SignInAsync(user);

        });
        
        builder.WithTags(EndpointTags.Authentication);
    }
}

using Beacon.API.Services;
using Beacon.App.Exceptions;
using Beacon.Common.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Auth;

public sealed class Login : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<LoginRequest>("auth/login").WithTags(EndpointTags.Authentication);
    }

    internal sealed class Handler : IRequestHandler<LoginRequest>
    {
        private readonly BeaconAuthenticationService _authService;
        private readonly HttpContext _httpContext;

        public Handler(BeaconAuthenticationService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task Handle(LoginRequest request, CancellationToken ct)
        {
            var user = await _authService.AuthenticateAsync(request.EmailAddress, request.Password, ct);

            if (user.Identity?.IsAuthenticated is not true)
                throw new BeaconValidationException(nameof(LoginRequest.EmailAddress), "Email address or password is invalid.");

            await _httpContext.SignInAsync(user);
        }
    }
}

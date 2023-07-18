using Beacon.API.Exceptions;
using Beacon.API.Services;
using Beacon.Common.Requests.Auth;
using MediatR;
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
        private readonly ISignInManager _signInManager;

        public Handler(BeaconAuthenticationService authService, ISignInManager signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }

        public async Task Handle(LoginRequest request, CancellationToken ct)
        {
            var identity = await _authService.AuthenticateAsync(request.EmailAddress, request.Password, ct);

            if (!identity.IsAuthenticated)
                throw new BeaconValidationException(nameof(LoginRequest.EmailAddress), "Email address or password is invalid.");

            await _signInManager.SignInAsync(new(identity));
        }
    }
}

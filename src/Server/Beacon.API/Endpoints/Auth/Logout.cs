using Beacon.API.Services;
using Beacon.Common.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Auth;

public sealed class Logout : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("auth/logout", new LogoutRequest()).WithTags(EndpointTags.Authentication);
    }

    internal sealed class Handler : IRequestHandler<LogoutRequest>
    {
        private readonly ISignInManager _signInManager;

        public Handler(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task Handle(LogoutRequest request, CancellationToken ct)
        {
            await _signInManager.SignOutAsync();
        }
    }
}

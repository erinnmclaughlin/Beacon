using Beacon.Common.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
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
        private readonly HttpContext _httpContext;

        public Handler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task Handle(LogoutRequest request, CancellationToken ct)
        {
            await _httpContext.SignOutAsync();
        }
    }
}

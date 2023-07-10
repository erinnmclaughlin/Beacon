using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Auth;

public sealed class GetSessionContext : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("users/current", new GetSessionContextRequest()).WithTags(EndpointTags.Authentication);
    }

    internal sealed class Handler : IRequestHandler<GetSessionContextRequest, SessionContext>
    {
        private readonly ISessionContext _context;

        public Handler(ISessionContext context)
        {
            _context = context;
        }

        public Task<SessionContext> Handle(GetSessionContextRequest request, CancellationToken ct)
        {
            return Task.FromResult(new SessionContext
            {
                CurrentUser = _context.CurrentUser,
                CurrentLab = _context.CurrentLab
            });
        }
    }
}

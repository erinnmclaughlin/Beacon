using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using ErrorOr;

namespace Beacon.API.Endpoints.Auth;

internal sealed class GetSessionContextHandler : IBeaconRequestHandler<GetSessionContextRequest, SessionContext>
{
    private readonly ISessionContext _context;

    public GetSessionContextHandler(ISessionContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<SessionContext>> Handle(GetSessionContextRequest request, CancellationToken ct)
    {
        return await Task.FromResult(new SessionContext
        {
            CurrentUser = _context.CurrentUser,
            CurrentLab = _context.CurrentLab
        });
    }
}

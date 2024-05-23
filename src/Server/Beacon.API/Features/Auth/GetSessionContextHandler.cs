using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using ErrorOr;

namespace Beacon.API.Features.Auth;

internal sealed class GetSessionContextHandler(ISessionContext context) : IBeaconRequestHandler<GetSessionContextRequest, SessionContext>
{
    private readonly ISessionContext _context = context;

    public async Task<ErrorOr<SessionContext>> Handle(GetSessionContextRequest request, CancellationToken ct)
    {
        return await Task.FromResult(new SessionContext
        {
            CurrentUser = _context.CurrentUser,
            CurrentLab = _context.CurrentLab
        });
    }
}

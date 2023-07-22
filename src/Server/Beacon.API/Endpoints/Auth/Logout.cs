using Beacon.API.Services;
using Beacon.Common.Requests.Auth;
using ErrorOr;

namespace Beacon.API.Endpoints.Auth;

internal sealed class LogoutHandler : IBeaconRequestHandler<LogoutRequest>
{
    private readonly ISignInManager _signInManager;

    public LogoutHandler(ISignInManager signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<ErrorOr<Success>> Handle(LogoutRequest request, CancellationToken ct)
    {
        await _signInManager.SignOutAsync();
        return Result.Success;
    }
}

using Beacon.API.Services;
using Beacon.Common.Requests.Auth;
using ErrorOr;

namespace Beacon.API.Features.Auth;

internal sealed class LogoutHandler(ISignInManager signInManager) : IBeaconRequestHandler<LogoutRequest>
{
    private readonly ISignInManager _signInManager = signInManager;

    public async Task<ErrorOr<Success>> Handle(LogoutRequest request, CancellationToken ct)
    {
        await _signInManager.SignOutAsync();
        return Result.Success;
    }
}

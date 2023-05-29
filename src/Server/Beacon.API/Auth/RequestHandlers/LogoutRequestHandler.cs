using Beacon.API.Auth.Services;
using Beacon.Common;
using Beacon.Common.Auth.Requests;
using ErrorOr;

namespace Beacon.API.Auth.RequestHandlers;
internal class LogoutRequestHandler : IApiRequestHandler<LogoutRequest, Success>
{
    private readonly ISignInManager _signInManager;

    public LogoutRequestHandler(ISignInManager signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<ErrorOr<Success>> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        return Result.Success;
    }
}

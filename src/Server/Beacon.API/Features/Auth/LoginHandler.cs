using Beacon.API.Services;
using Beacon.Common.Requests.Auth;
using ErrorOr;

namespace Beacon.API.Features.Auth;

internal sealed class LoginHandler(BeaconAuthenticationService authService, ISignInManager signInManager) : IBeaconRequestHandler<LoginRequest>
{
    private readonly BeaconAuthenticationService _authService = authService;
    private readonly ISignInManager _signInManager = signInManager;

    public async Task<ErrorOr<Success>> Handle(LoginRequest request, CancellationToken ct)
    {
        var identity = await _authService.AuthenticateAsync(request.EmailAddress, request.Password, ct);

        if (!identity.IsAuthenticated)
        {
            return Error.Validation(nameof(LoginRequest.EmailAddress), "Email address or password is invalid.");
        }

        await _signInManager.SignInAsync(new(identity));
        return Result.Success;
    }
}

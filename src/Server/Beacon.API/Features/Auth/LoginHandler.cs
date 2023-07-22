using Beacon.API.Services;
using Beacon.Common.Requests.Auth;
using ErrorOr;

namespace Beacon.API.Features.Auth;

internal sealed class LoginHandler : IBeaconRequestHandler<LoginRequest>
{
    private readonly BeaconAuthenticationService _authService;
    private readonly ISignInManager _signInManager;

    public LoginHandler(BeaconAuthenticationService authService, ISignInManager signInManager)
    {
        _authService = authService;
        _signInManager = signInManager;
    }

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

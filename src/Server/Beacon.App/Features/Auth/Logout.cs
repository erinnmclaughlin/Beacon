using Beacon.App.Services;
using MediatR;

namespace Beacon.App.Features.Auth;

public static class Logout
{
    public sealed record Command : IRequest;

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ISignInManager _signInManager;

        public Handler(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
        }
    }
}

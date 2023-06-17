using Beacon.App.Services;
using MediatR;

namespace Beacon.App.Features.Auth;

public static class Logout
{
    public sealed record Command : IRequest;

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ISessionManager _currentSession;

        public Handler(ISessionManager currentSession)
        {
            _currentSession = currentSession;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _currentSession.SignOutAsync();
        }
    }
}

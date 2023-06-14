using Beacon.App.Services;
using MediatR;

namespace Beacon.App.Features.Laboratories.Commands;

public static class LogoutOfLaboratory
{
    public sealed record Command : IRequest;

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ISessionManager _sessionManager;

        public Handler(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Task Handle(Command request, CancellationToken cancellationToken)
        {
            return _sessionManager.ClearCurrentLabAsync();
        }
    }
}

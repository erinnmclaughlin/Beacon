using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class LoginToLaboratory
{
    public sealed record Command(Guid LabId) : IRequest;

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ISessionManager _currentSession;
        private readonly IQueryService _queryService;

        public Handler(ISessionManager currentSession, IQueryService queryService)
        {
            _currentSession = currentSession;
            _queryService = queryService;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _currentSession.UserId;

            var membershipInfo = await _queryService
                .QueryFor<Membership>()
                .Where(m => m.MemberId == userId && m.LaboratoryId == request.LabId)
                .Select(m => new { m.MembershipType })
                .SingleAsync(cancellationToken);

            await _currentSession.SetCurrentLabAsync(request.LabId, membershipInfo.MembershipType);
        }
    }
}

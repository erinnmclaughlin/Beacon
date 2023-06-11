using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class SetCurrentLaboratory
{
    public sealed record Command(Guid LabId) : IRequest;

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly IQueryService _queryService;
        private readonly ISessionManager _sessionManager;

        public Handler(IQueryService queryService, ISessionManager sessionManager)
        {
            _queryService = queryService;
            _sessionManager = sessionManager;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _sessionManager.UserId;

            var membershipInfo = await _queryService
                .QueryFor<LaboratoryMembership>()
                .Where(m => m.MemberId == userId && m.LaboratoryId == request.LabId)
                .Select(m => new { m.MembershipType })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new LaboratoryNotFoundException(request.LabId);

            _sessionManager.SetCurrentLab(request.LabId, membershipInfo.MembershipType);
        }
    }
}

using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Auth;

public static class GetSessionInfo
{
    public sealed record Query : IRequest<SessionInfoDto>;

    internal sealed class Handler : IRequestHandler<Query, SessionInfoDto>
    {
        private readonly IQueryService _queryService;
        private readonly ISessionManager _sessionManager;

        public Handler(IQueryService queryService, ISessionManager sessionManager)
        {
            _queryService = queryService;
            _sessionManager = sessionManager;
        }

        public async Task<SessionInfoDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUserId = _sessionManager.UserId;
            var currentLabId = _sessionManager.LabId;

            var currentUser = await _queryService
                .QueryFor<User>()
                .Where(u => u.Id == currentUserId)
                .Select(u => new SessionInfoDto.UserDto(u.Id, u.DisplayName))
                .FirstAsync(cancellationToken);

            var currentLab = await _queryService
                .QueryFor<LaboratoryMembership>()
                .Where(m => m.LaboratoryId == currentLabId && m.MemberId == currentUserId)
                .Select(m => new SessionInfoDto.LabDto(m.Laboratory.Id, m.Laboratory.Name, m.MembershipType))
                .FirstOrDefaultAsync(cancellationToken);

            return new SessionInfoDto(currentUser, currentLab);
        }
    }
}

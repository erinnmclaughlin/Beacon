using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Laboratories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories.Queries;

public static class GetMyLaboratoriesFeature
{
    public sealed record Query : IRequest<LaboratoryDto[]>;

    internal sealed class Handler : IRequestHandler<Query, LaboratoryDto[]>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IQueryService _queryService;

        public Handler(ICurrentUser currentUser, IQueryService queryService)
        {
            _currentUser = currentUser;
            _queryService = queryService;
        }

        public async Task<LaboratoryDto[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            return await _queryService
                .QueryFor<LaboratoryMembership>()
                .Where(m => m.MemberId == currentUserId)
                .Select(m => new LaboratoryDto
                {
                    Id = m.Laboratory.Id,
                    Name = m.Laboratory.Name,
                    MyMembershipType = m.MembershipType
                })
                .ToArrayAsync(cancellationToken);
        }
    }
}

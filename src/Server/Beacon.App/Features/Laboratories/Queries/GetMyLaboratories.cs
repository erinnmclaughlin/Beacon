using Beacon.App.Services;
using Beacon.Common.Laboratories.Enums;
using Beacon.Common.Laboratories.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories.Queries;

public static class GetMyLaboratoriesFeature
{
    public sealed record Query : IRequest<GetMyLaboratories.Laboratory[]>;

    internal sealed class Handler : IRequestHandler<Query, GetMyLaboratories.Laboratory[]>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IQueryService _queryService;

        public Handler(ICurrentUser currentUser, IQueryService queryService)
        {
            _currentUser = currentUser;
            _queryService = queryService;
        }

        public async Task<GetMyLaboratories.Laboratory[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId;

            var data = await _queryService
                .QueryFor<Entities.LaboratoryMembership>()
                .Where(m => m.MemberId == currentUserId)
                .Select(m => new
                {
                    m.Laboratory.Id,
                    m.Laboratory.Name,
                    m.MembershipType,
                    Admin = m.Laboratory.Memberships.First(m => m.MembershipType == LaboratoryMembershipType.Admin).Member
                })
                .ToListAsync(cancellationToken);

            return data.Select(l => new GetMyLaboratories.Laboratory
            {
                Id = l.Id,
                Name = l.Name,
                MembershipType = l.MembershipType,
                Admin = new GetMyLaboratories.LaboratoryMember { Id = l.Admin.Id, DisplayName = l.Admin.DisplayName }
            }).ToArray();
        }
    }
}

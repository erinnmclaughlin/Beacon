using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Laboratories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Memberships;

public static class GetMembers
{
    public sealed record Query : IRequest<LaboratoryMemberDto[]>;

    internal sealed class Handler : IRequestHandler<Query, LaboratoryMemberDto[]>
    {
        private readonly ICurrentLab _currentLab;
        private readonly IQueryService _queryService;

        public Handler(ICurrentLab currentLab, IQueryService queryService)
        {
            _currentLab = currentLab;
            _queryService = queryService;
        }

        public async Task<LaboratoryMemberDto[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var labId = _currentLab.LabId;

            return await _queryService
                .QueryFor<Membership>()
                .Where(m => m.LaboratoryId == labId)
                .Select(m => new LaboratoryMemberDto
                {
                    Id = m.Member.Id,
                    DisplayName = m.Member.DisplayName,
                    EmailAddress = m.Member.EmailAddress,
                    MembershipType = m.MembershipType
                })
                .ToArrayAsync(cancellationToken);
        }
    }
}

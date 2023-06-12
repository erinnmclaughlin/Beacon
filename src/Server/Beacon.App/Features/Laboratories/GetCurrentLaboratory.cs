using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Laboratories.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class GetCurrentLaboratory
{
    public sealed record Query : IRequest<Response>;

    public sealed record Response(LaboratoryDto Laboratory);

    public sealed record LaboratoryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required LaboratoryMembershipType CurrentUserMembershipType { get; init; }
        public required List<MemberDto> Members { get; init; }
    }

    public sealed record MemberDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required string EmailAddress { get; init; }
        public required LaboratoryMembershipType MembershipType { get; init; }
    }

    public sealed class QueryHandler : IRequestHandler<Query, Response>
    {
        private readonly ICurrentLab _currentLab;
        private readonly IQueryService _queryService;

        public QueryHandler(ICurrentLab currentLab, IQueryService queryService)
        {
            _currentLab = currentLab;
            _queryService = queryService;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var labId = _currentLab.LabId;
            var membershipType = _currentLab.MembershipType;

            var lab = await _queryService
                .QueryFor<Laboratory>()
                .Where(l => l.Id == labId)
                .Select(l => new LaboratoryDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    CurrentUserMembershipType = membershipType,
                    Members = l.Memberships.Select(m => new MemberDto
                    {
                        Id = m.Member.Id,
                        DisplayName = m.Member.DisplayName,
                        EmailAddress = m.Member.EmailAddress,
                        MembershipType = m.MembershipType
                    }).ToList()
                })
                .AsSplitQuery()
                .FirstAsync(cancellationToken);

            return new Response(lab);
        }
    }
}

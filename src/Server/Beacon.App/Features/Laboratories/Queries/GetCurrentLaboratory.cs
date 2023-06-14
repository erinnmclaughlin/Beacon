using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Laboratories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories.Queries;

public static class GetCurrentLaboratory
{
    public sealed record Query : IRequest<LaboratoryDto>;

    public sealed class QueryHandler : IRequestHandler<Query, LaboratoryDto>
    {
        private readonly ICurrentLab _currentLab;
        private readonly IQueryService _queryService;

        public QueryHandler(ICurrentLab currentLab, IQueryService queryService)
        {
            _currentLab = currentLab;
            _queryService = queryService;
        }

        public async Task<LaboratoryDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var labId = _currentLab.LabId;
            var membershipType = _currentLab.MembershipType;

            return await _queryService
                .QueryFor<Laboratory>()
                .Where(l => l.Id == labId)
                .Select(l => new LaboratoryDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    MyMembershipType = membershipType
                })
                .FirstAsync(cancellationToken);
        }
    }
}

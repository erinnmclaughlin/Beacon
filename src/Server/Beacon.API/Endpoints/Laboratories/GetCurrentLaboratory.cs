using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class GetCurrentLaboratoryHandler : IBeaconRequestHandler<GetCurrentLaboratoryRequest, LaboratoryDto>
{
    private readonly BeaconDbContext _dbContext;
    private readonly ILabContext _labContext;

    public GetCurrentLaboratoryHandler(BeaconDbContext dbContext, ILabContext labContext)
    {
        _dbContext = dbContext;
        _labContext = labContext;
    }

    public async Task<ErrorOr<LaboratoryDto>> Handle(GetCurrentLaboratoryRequest request, CancellationToken ct)
    {
        return await _dbContext.Laboratories
            .Where(x => x.Id == _labContext.CurrentLab.Id)
            .Select(x => new LaboratoryDto
            {
                Id = x.Id,
                Name = x.Name,
                MemberCount = x.Memberships.Count
            })
            .SingleAsync(ct);
    }
}

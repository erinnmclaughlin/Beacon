using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Instruments;

internal sealed class GetLaboratoryInstrumentsHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<GetLaboratoryInstrumentsRequest, LaboratoryInstrumentDto[]>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<LaboratoryInstrumentDto[]>> Handle(GetLaboratoryInstrumentsRequest request, CancellationToken ct)
    {
        var query = _dbContext.LaboratoryInstruments.AsQueryable();

        if (request.IgnoredInstrumentIds is { Count: > 0 } ignoredIds)
        {
            query = query.Where(i => !ignoredIds.Contains(i.Id));
        }

        return await query
            .Select(i => new LaboratoryInstrumentDto
            {
                Id = i.Id,
                InstrumentType = i.InstrumentType,
                SerialNumber = i.SerialNumber
            })
            .ToArrayAsync(ct);
    }
}

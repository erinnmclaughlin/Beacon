using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Instruments;

internal sealed class GetLaboratoryInstrumentsHandler : IBeaconRequestHandler<GetLaboratoryInstrumentsRequest, LaboratoryInstrumentDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetLaboratoryInstrumentsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryInstrumentDto[]>> Handle(GetLaboratoryInstrumentsRequest request, CancellationToken ct)
    {
        return await _dbContext.LaboratoryInstruments
            .Select(i => new LaboratoryInstrumentDto
            {
                Id = i.Id,
                InstrumentType = i.InstrumentType,
                SerialNumber = i.SerialNumber
            })
            .ToArrayAsync(ct);
    }
}

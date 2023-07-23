using Beacon.API.Persistence;
using Beacon.Common.Requests.Instruments;
using ErrorOr;

namespace Beacon.API.Features.Instruments;

internal sealed class CreateLaboratoryInstrumentHandler : IBeaconRequestHandler<CreateLaboratoryInstrumentRequest>
{
    private readonly BeaconDbContext _dbContext;

    public CreateLaboratoryInstrumentHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(CreateLaboratoryInstrumentRequest request, CancellationToken ct)
    {
        _dbContext.LaboratoryInstruments.Add(new()
        {
            InstrumentType = request.InstrumentType,
            SerialNumber = request.SerialNumber
        });

        await _dbContext.SaveChangesAsync(ct);

        return Result.Success;
    }
}

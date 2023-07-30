using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Events;

internal sealed class UnassociateInstrumentFromProjectEventHandler : IBeaconRequestHandler<UnassociateInstrumentFromProjectEventRequest>
{
    private readonly BeaconDbContext _dbContext;

    public UnassociateInstrumentFromProjectEventHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(UnassociateInstrumentFromProjectEventRequest request, CancellationToken cancellationToken)
    {
        await _dbContext.Set<LaboratoryInstrumentUsage>()
            .Where(x => x.ProjectEventId == request.ProjectEventId && x.InstrumentId == request.InstrumentId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success;
    }
}

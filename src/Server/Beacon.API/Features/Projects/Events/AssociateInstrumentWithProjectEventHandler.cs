using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Events;

internal sealed class AssociateInstrumentWithProjectEventHandler : IBeaconRequestHandler<AssociateInstrumentWithProjectEventRequest>
{
    private readonly BeaconDbContext _dbContext;

    public AssociateInstrumentWithProjectEventHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(AssociateInstrumentWithProjectEventRequest request, CancellationToken ct)
    {
        var dbSet = _dbContext.Set<LaboratoryInstrumentUsage>();

        var associationExists = await dbSet.AnyAsync(x =>
            x.ProjectEventId == request.ProjectEventId && 
            x.InstrumentId == request.InstrumentId,
            ct);

        if (!associationExists)
        {
            dbSet.Add(new LaboratoryInstrumentUsage
            {
                ProjectEventId = request.ProjectEventId,
                InstrumentId = request.InstrumentId
            });
            await _dbContext.SaveChangesAsync(ct);
        }

        return Result.Success;
    }
}

using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Events;

internal sealed class LinkInstrumentToProjectEventHandler : IBeaconRequestHandler<LinkInstrumentToProjectEventRequest>
{
    private readonly BeaconDbContext _dbContext;

    public LinkInstrumentToProjectEventHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(LinkInstrumentToProjectEventRequest request, CancellationToken ct)
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

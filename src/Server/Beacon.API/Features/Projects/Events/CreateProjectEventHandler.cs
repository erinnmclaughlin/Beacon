using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Events;

internal sealed class CreateProjectEventHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<CreateProjectEventRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Success>> Handle(CreateProjectEventRequest request, CancellationToken ct)
    {
        var projectEvent = new ProjectEvent
        {
            Title = request.Title,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
            ProjectId = request.ProjectId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd,
            AssociatedInstruments = request.InstrumentIds.Any()
                ? await _dbContext.LaboratoryInstruments.Where(i => request.InstrumentIds.Contains(i.Id)).ToListAsync(ct)
                : new()
        };

        if (request.InstrumentIds is { Count: > 0 } instrumentIds)
            projectEvent.AssociatedInstruments.AddRange(await _dbContext.LaboratoryInstruments.Where(i => instrumentIds.Contains(i.Id)).ToListAsync(ct));


        _dbContext.ProjectEvents.Add(projectEvent);
        await _dbContext.SaveChangesAsync(ct);

        return Result.Success;
    }
}

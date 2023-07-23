using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Events;
using ErrorOr;

namespace Beacon.API.Features.Projects.Events;

internal sealed class CreateProjectEventHandler : IBeaconRequestHandler<CreateProjectEventRequest>
{
    private readonly BeaconDbContext _dbContext;

    public CreateProjectEventHandler(BeaconDbContext dbContext  )
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(CreateProjectEventRequest request, CancellationToken ct)
    {
        _dbContext.ProjectEvents.Add(new ProjectEvent
        {
            Title = request.Title,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
            ProjectId = request.ProjectId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd
        });

        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}

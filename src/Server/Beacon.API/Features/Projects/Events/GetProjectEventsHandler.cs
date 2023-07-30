using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Events;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Beacon.API.Features.Projects.Events;

internal sealed class GetProjectEventsHandler : IBeaconRequestHandler<GetProjectEventsRequest, ProjectEventDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetProjectEventsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProjectEventDto[]>> Handle(GetProjectEventsRequest request, CancellationToken ct)
    {
        return await _dbContext.ProjectEvents
            .Where(GetFilter(request))
            .Select(x => new ProjectEventDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ScheduledStart = x.ScheduledStart,
                ScheduledEnd = x.ScheduledEnd,
                AssociatedInstruments = x.AssociatedInstruments
                    .Select(i => new LaboratoryInstrumentDto
                    {
                        Id = i.Id,
                        InstrumentType = i.InstrumentType,
                        SerialNumber = i.SerialNumber
                    })
                    .ToArray()
            })
            .AsNoTracking()
            .ToArrayAsync(ct);
    }

    private static Expression<Func<ProjectEvent, bool>> GetFilter(GetProjectEventsRequest request)
    {
        var expressions = new List<Expression<Func<ProjectEvent, bool>>>()
        {
            x => x.ProjectId == request.ProjectId
        };

        if (request.MinStart is { } minStart)
        {
            expressions.Add(x => x.ScheduledStart >= minStart);
        }

        if (request.MinEnd is { } minEnd)
        {
            expressions.Add(x => x.ScheduledEnd >= minEnd);
        }

        if (request.MaxStart is { } maxStart)
        {
            expressions.Add(x => x.ScheduledStart <= maxStart);
        }

        if (request.MaxEnd is { } maxEnd)
        {
            expressions.Add(x => x.ScheduledEnd <= maxEnd);
        }

        return expressions.GetFilter();
    }
}

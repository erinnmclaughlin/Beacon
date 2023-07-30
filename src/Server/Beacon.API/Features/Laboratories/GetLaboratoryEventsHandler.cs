using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Beacon.API.Features.Laboratories;

internal sealed class GetLaboratoryEventsHandler : IBeaconRequestHandler<GetLaboratoryEventsRequest, LaboratoryEventDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetLaboratoryEventsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryEventDto[]>> Handle(GetLaboratoryEventsRequest request, CancellationToken ct)
    {
        return await _dbContext.ProjectEvents
            .Where(GetFilter(request))
            .Select(p => new LaboratoryEventDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ScheduledStart = p.ScheduledStart,
                ScheduledEnd = p.ScheduledEnd,
                ProjectCode = p.Project.ProjectCode
            })
            .AsNoTracking()
            .ToArrayAsync(ct);
    }

    private static Expression<Func<ProjectEvent, bool>> GetFilter(GetLaboratoryEventsRequest request)
    {
        var expressions = new List<Expression<Func<ProjectEvent, bool>>>();

        if (request.ProjectIds is { Count: > 0 } projectIds)
        {
            expressions.Add(x => projectIds.Contains(x.ProjectId));
        }

        if (request.MinStart is { } minStart)
        {
            expressions.Add(x => x.ScheduledStart >= minStart);
        }

        if (request.MaxStart is { } maxStart)
        {
            expressions.Add(x => x.ScheduledStart <= maxStart);
        }

        if (request.MinEnd is { } minEnd)
        {
            expressions.Add(x => x.ScheduledEnd >= minEnd);
        }

        if (request.MaxEnd is { } maxEnd)
        {
            expressions.Add(x => x.ScheduledEnd <= maxEnd);
        }

        return expressions.GetFilter();
    }
}

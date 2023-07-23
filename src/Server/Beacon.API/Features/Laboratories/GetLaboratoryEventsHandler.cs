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

        if (request.MinDate?.ToDateTime(TimeOnly.MinValue) is { } minDate)
        {
            expressions.Add(x => x.ScheduledEnd >= minDate);
        }

        if (request.MaxDate?.ToDateTime(TimeOnly.MinValue) is { } maxDate)
        {
            expressions.Add(x => x.ScheduledStart <= maxDate);
        }

        return expressions.GetFilter();
    }
}

using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Beacon.API.Features.Laboratories;

internal sealed class GetLaboratoryEventsHandler : IBeaconRequestHandler<GetLaboratoryEventsRequest, PagedList<LaboratoryEventDto>>
{
    private readonly BeaconDbContext _dbContext;

    public GetLaboratoryEventsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PagedList<LaboratoryEventDto>>> Handle(GetLaboratoryEventsRequest request, CancellationToken ct)
    {
        var result = await _dbContext.ProjectEvents
            .Where(GetFilter(request))
            .Select(p => new LaboratoryEventDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ScheduledStart = p.ScheduledStart,
                ScheduledEnd = p.ScheduledEnd,
                ProjectCode = p.Project.ProjectCode,
                AssociatedInstruments = p.AssociatedInstruments
                    .Select(i => new LaboratoryInstrumentDto
                    {
                        Id = i.Id,
                        InstrumentType = i.InstrumentType,
                        SerialNumber = i.SerialNumber
                    })
                    .ToArray()
            })
            .OrderBy(p => p.ScheduledStart)
                .ThenBy(p => p.Title)
                    .ThenBy(p => p.Id)
            .AsNoTracking()
            .ToPagedListAsync(request, ct);

        return result;
    }

    private static Expression<Func<ProjectEvent, bool>> GetFilter(GetLaboratoryEventsRequest request)
    {
        var filterBuilder = new FilterBuilder<ProjectEvent>();
        
        if (request.ProjectIds is { Count: > 0 } projectIds)
        {
            filterBuilder.Add(x => projectIds.Contains(x.ProjectId));
        }

        if (request.MinStart is { } minStart)
        {
            filterBuilder.Add(x => x.ScheduledStart >= minStart);
        }

        if (request.MaxStart is { } maxStart)
        {
            filterBuilder.Add(x => x.ScheduledStart <= maxStart);
        }

        if (request.MinEnd is { } minEnd)
        {
            filterBuilder.Add(x => x.ScheduledEnd >= minEnd);
        }

        if (request.MaxEnd is { } maxEnd)
        {
            filterBuilder.Add(x => x.ScheduledEnd <= maxEnd);
        }

        return filterBuilder.Build();
    }
}

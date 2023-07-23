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
                ScheduledEnd = x.ScheduledEnd
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

        if (request.MinDate?.ToDateTime(TimeOnly.MinValue) is { } minDate)
        {
            expressions.Add(x => x.ScheduledEnd >= minDate);
        }

        if (request.MaxDate?.ToDateTime(TimeOnly.MinValue) is { } maxDate)
        {
            expressions.Add(x => x.ScheduledStart <= maxDate);
        }

        return Extensions.GetFilter(expressions);
    }
}

file static class Extensions
{
    public static Expression<Func<T, bool>> GetFilter<T>(IEnumerable<Expression<Func<T, bool>>> expressions)
    {
        if (!expressions.Any())
            return x => true;

        var filter = expressions.First();

        foreach (var exp in expressions.Skip(1))
        {
            filter = filter.AndAlso(exp);
        }

        return filter;
    }

    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        // need to detect whether they use the same
        // parameter instance; if not, they need fixing
        var param = expr1.Parameters[0];

        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            // simple version
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, expr2.Body), param);
        }

        // otherwise, keep expr1 "as is" and invoke expr2
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, Expression.Invoke(expr2, param)), param);
    }
}
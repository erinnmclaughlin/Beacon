using System.Linq.Expressions;
using Beacon.Common;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Extensions;

public static class QueryableExtensions
{
    public static async Task<ErrorOr<T>> SingleOrErrorAsync<T>(this IQueryable<T> source, CancellationToken ct)
    {
        var valueOrNull = await source.SingleOrDefaultAsync(ct);
        return valueOrNull is null ? Error.NotFound() : valueOrNull;
    }

    public static async Task<ErrorOr<T>> SingleOrErrorAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, CancellationToken ct)
    {
        return await source.Where(predicate).SingleOrErrorAsync(ct);
    }
    
    public static Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, IPaginated query, CancellationToken ct)
    {
        return source.ToPagedListAsync(query.PageNumber, query.PageSize, ct);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var count = await source.CountAsync(ct);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArrayAsync(ct);
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}

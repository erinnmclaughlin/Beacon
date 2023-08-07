using Beacon.Common;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API;

public static class QueryableExtensions
{
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

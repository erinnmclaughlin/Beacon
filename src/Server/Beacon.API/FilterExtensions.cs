using System.Linq.Expressions;

namespace Beacon.API;

public class FilterBuilder<T>
{
    private readonly List<Expression<Func<T, bool>>> _filters = new();

    public FilterBuilder<T> Add(Expression<Func<T, bool>> filter)
    {
        _filters.Add(filter);
        return this;
    }

    public Expression<Func<T, bool>> Build()
    {
        return _filters.GetFilter();
    }
}

public static class FilterExtensions
{
    public static Expression<Func<T, bool>> GetFilter<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
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
using System.Linq.Expressions;

namespace Beacon.API.Extensions;

public static class FilterExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var (param1, param2) = (expr1.Parameters[0], expr2.Parameters[0]);
        var body = ReferenceEquals(param1, param2) ? expr2.Body : Expression.Invoke(expr2, param1);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, body), param1);
    }
}
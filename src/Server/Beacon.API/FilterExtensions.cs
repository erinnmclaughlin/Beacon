using System.Linq.Expressions;

namespace Beacon.API;

public static class FilterExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var param = expr1.Parameters[0];

        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, expr2.Body), param);
        }

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, Expression.Invoke(expr2, param)), param);
    }
}
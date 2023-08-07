using System.Linq.Expressions;

namespace Beacon.API;

public class FilterBuilder<T>
{
    private Expression<Func<T, bool>>? _filter;

    public FilterBuilder<T> Add(Expression<Func<T, bool>> filter)
    {
        _filter = _filter == null ? filter : _filter.AndAlso(filter);
        return this;
    }

    public Expression<Func<T, bool>> Build()
    {
        return _filter ?? (x => true);
    }
}
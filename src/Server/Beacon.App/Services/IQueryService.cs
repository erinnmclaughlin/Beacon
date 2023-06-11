namespace Beacon.App.Services;

public interface IQueryService
{
    IQueryable<T> QueryFor<T>() where T : class;
}

namespace Beacon.API.App.Services;

public interface IRepository<T> where T : class
{
    void Add(T entity);
    void AddRange(params T[] entities);
    IQueryable<T> AsQueryable();
    void Remove(T entity);
    void RemoveRange(params T[] entities);
}
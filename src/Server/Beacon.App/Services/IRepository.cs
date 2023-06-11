namespace Beacon.App.Services;

public interface IRepository<T> where T : class
{
    void Add(T entity);
} 
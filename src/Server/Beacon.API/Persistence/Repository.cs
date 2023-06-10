using Beacon.API.App.Services;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Persistence;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _dbContext;

    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(T entity) => _dbContext.Set<T>().Add(entity);
    public void AddRange(params T[] entities) => _dbContext.Set<T>().AddRange(entities);

    public void Remove(T entity) => _dbContext.Set<T>().Remove(entity);
    public void RemoveRange(params T[] entities) => _dbContext.Set<T>().RemoveRange(entities);

    public IQueryable<T> AsQueryable() => _dbContext.Set<T>();
}

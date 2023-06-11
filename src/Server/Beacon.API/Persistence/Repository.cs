using Beacon.App.Services;
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
}

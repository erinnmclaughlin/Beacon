namespace Beacon.API.App.Services;

public interface IUnitOfWork
{
    IRepository<T> GetRepository<T>() where T : class;
    IQueryable<T> QueryFor<T>(bool enableChangeTracking = false) where T : class;
    Task<int> SaveChangesAsync(CancellationToken ct);
}

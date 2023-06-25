namespace Beacon.Common;

public interface ILabContext
{
    Task<Guid> GetLaboratoryId();
    Task SetLaboratoryId(Guid id);
}

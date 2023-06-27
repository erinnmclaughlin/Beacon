using Beacon.Common.Models;

namespace Beacon.Common.Services;

public interface ILabContext
{
    Guid LaboratoryId { get; }
    Task<LaboratoryMembershipType?> GetMembershipTypeAsync(Guid userId, CancellationToken ct = default);
}

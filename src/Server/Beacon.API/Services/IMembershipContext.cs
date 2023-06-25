using Beacon.Common.Models;

namespace Beacon.API.Services;
public interface IMembershipContext
{
    Task<LaboratoryMembershipType?> GetMembershipTypeAsync(CancellationToken ct = default);
}
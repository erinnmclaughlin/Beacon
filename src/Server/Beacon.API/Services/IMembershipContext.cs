using Beacon.Common.Memberships;

namespace Beacon.API.Services;
public interface IMembershipContext
{
    Task<LaboratoryMembershipType?> GetMembershipTypeAsync(CancellationToken ct = default);
}
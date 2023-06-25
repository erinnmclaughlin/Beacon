using Beacon.Common.Models;

namespace Beacon.Common.Services;

public interface ILabContext
{
    Guid LaboratoryId { get; }
    LaboratoryMembershipType? MembershipType { get; }
}

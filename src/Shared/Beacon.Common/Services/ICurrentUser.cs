using Beacon.Common.Models;

namespace Beacon.Common.Services;

public interface ICurrentUser
{
    Guid UserId { get; }
    LaboratoryMembershipType? MembershipType { get; }
}

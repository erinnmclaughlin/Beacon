using Beacon.Common.Memberships;

namespace Beacon.App.Services;

public interface ICurrentLab
{
    Guid LabId { get; }
    LaboratoryMembershipType MembershipType { get; }
}

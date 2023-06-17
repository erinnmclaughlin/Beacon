using Beacon.Common.Laboratories;

namespace Beacon.App.Services;

public interface ICurrentLab
{
    Guid LabId { get; }
    LaboratoryMembershipType MembershipType { get; }
}

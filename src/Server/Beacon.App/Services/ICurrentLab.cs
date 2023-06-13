using Beacon.Common.Laboratories.Enums;

namespace Beacon.App.Services;

public interface ICurrentLab
{
    Guid LabId { get; }
    LaboratoryMembershipType MembershipType { get; }
}

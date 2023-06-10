namespace Beacon.API.Domain.Exceptions;

public class LaboratoryMembershipNotFoundException : BeaconException
{
    public Guid LabId { get; }
    public Guid MembershipId { get; }

    public LaboratoryMembershipNotFoundException(Guid labId, Guid memberId)
        : base(BeaconExceptionType.NotFound, "Membership not found.")
    {
        LabId = labId;
        MembershipId = memberId;
    }
}

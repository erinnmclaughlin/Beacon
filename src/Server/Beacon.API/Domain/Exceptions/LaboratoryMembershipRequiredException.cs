namespace Beacon.API.Domain.Exceptions;

public class LaboratoryMembershipRequiredException : BeaconException
{
    public Guid LaboratoryId { get; }

    public LaboratoryMembershipRequiredException(Guid laboratoryId, string message = "The current user is not a member of this lab.")
        : base(BeaconExceptionType.NotAuthorized, message)
    {
        LaboratoryId = laboratoryId;
    }
}

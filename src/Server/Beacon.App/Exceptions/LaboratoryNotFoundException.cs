namespace Beacon.App.Exceptions;

public class LaboratoryNotFoundException : BeaconException
{
    public Guid LaboratoryId { get; }

    public LaboratoryNotFoundException(Guid labId)
        : base(BeaconExceptionType.NotFound, $"Laboratory with id {labId} was not found.")
    {
        LaboratoryId = labId;
    }
}

namespace Beacon.Common.Requests.Laboratories;

public sealed class SetCurrentLaboratoryRequest : BeaconRequest<SetCurrentLaboratoryRequest>
{
    public Guid? LaboratoryId { get; set; }
}

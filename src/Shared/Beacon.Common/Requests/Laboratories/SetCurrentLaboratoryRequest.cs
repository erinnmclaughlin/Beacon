namespace Beacon.Common.Requests.Laboratories;

public sealed class SetCurrentLaboratoryRequest : BeaconRequest<SetCurrentLaboratoryRequest>
{
    public required Guid? LaboratoryId { get; set; }
}

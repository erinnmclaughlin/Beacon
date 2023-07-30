namespace Beacon.Common.Requests.Projects.Events;

public sealed class AssociateInstrumentWithProjectEventRequest : BeaconRequest<AssociateInstrumentWithProjectEventRequest>
{
    public Guid ProjectEventId { get; set; }
    public Guid InstrumentId { get; set; }
}

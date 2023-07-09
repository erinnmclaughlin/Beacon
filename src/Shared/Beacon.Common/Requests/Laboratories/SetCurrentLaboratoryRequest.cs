using MediatR;

namespace Beacon.Common.Requests.Laboratories;

public sealed class SetCurrentLaboratoryRequest : IRequest
{
    public required Guid LaboratoryId { get; set; }
}

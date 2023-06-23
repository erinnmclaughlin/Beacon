using MediatR;

namespace Beacon.Common.Laboratories;

public class GetLaboratoryByIdRequest : IRequest<LaboratoryDto>
{
    public required Guid LaboratoryId { get; set; }
}

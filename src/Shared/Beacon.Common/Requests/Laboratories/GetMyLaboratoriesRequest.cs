using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Laboratories;

public sealed class GetMyLaboratoriesRequest : BeaconRequest<GetMyLaboratoriesRequest>, IRequest<LaboratoryDto[]> { }

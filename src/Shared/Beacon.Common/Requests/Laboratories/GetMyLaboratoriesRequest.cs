using Beacon.Common.Laboratories;
using MediatR;

namespace Beacon.Common.Requests.Laboratories;

public sealed class GetMyLaboratoriesRequest : IRequest<LaboratoryDto[]> { }

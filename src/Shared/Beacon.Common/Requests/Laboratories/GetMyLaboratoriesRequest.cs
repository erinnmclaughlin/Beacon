using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Laboratories;

public sealed class GetMyLaboratoriesRequest : IRequest<LaboratoryDto[]> { }

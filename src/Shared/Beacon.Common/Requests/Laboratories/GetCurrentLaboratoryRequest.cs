using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public class GetCurrentLaboratoryRequest : IRequest<LaboratoryDto> { }

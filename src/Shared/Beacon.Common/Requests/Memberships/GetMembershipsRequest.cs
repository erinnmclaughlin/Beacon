using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Requests.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetMembershipsRequest : IRequest<LaboratoryMemberDto[]> { }

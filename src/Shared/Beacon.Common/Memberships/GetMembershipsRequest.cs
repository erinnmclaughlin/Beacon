using MediatR;

namespace Beacon.Common.Memberships;

public class GetMembershipsRequest : IRequest<LaboratoryMemberDto[]>
{
    public required Guid LaboratoryId { get; set; }
}

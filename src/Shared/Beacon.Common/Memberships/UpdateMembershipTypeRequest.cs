namespace Beacon.Common.Memberships;

public class UpdateMembershipTypeRequest
{
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;
}

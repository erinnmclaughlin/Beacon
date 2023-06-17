namespace Beacon.Common.Laboratories.Requests;

public class UpdateMembershipTypeRequest
{
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;
}

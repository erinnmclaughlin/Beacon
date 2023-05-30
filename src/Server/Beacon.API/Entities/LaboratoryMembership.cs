namespace Beacon.API.Entities;

public class LaboratoryMembership
{
    public required LaboratoryMembershipType MembershipType { get; set; }
    public required Laboratory Laboratory { get; set; }
    public required User Member { get; set; }
}

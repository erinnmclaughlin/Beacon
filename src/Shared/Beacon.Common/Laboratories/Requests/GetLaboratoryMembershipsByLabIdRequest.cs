namespace Beacon.Common.Laboratories.Requests;

public class GetLaboratoryMembershipsByLabIdRequest : IApiRequest<List<LaboratoryMembershipDto>>
{
    public Guid LaboratoryId { get; set; }
}

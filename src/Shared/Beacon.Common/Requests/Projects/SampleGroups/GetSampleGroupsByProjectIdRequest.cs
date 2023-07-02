using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects.SampleGroups;

public class GetSampleGroupsByProjectIdRequest : IRequest<SampleGroupDto[]>
{
    public required Guid ProjectId { get; init; }
}

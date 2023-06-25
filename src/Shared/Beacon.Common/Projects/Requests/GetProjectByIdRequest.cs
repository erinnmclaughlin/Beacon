using MediatR;

namespace Beacon.Common.Projects.Requests;

public sealed class GetProjectByIdRequest : IRequest<ProjectDto?>
{
    public required Guid ProjectId { get; set; }
}

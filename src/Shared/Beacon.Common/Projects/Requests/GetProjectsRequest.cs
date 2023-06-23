using MediatR;

namespace Beacon.Common.Projects.Requests;

public sealed class GetProjectsRequest : IRequest<ProjectDto[]>
{
    public required Guid LaboratoryId { get; set; }
}

using MediatR;

namespace Beacon.Common.Projects.Requests;

public sealed class GetProjectByIdRequest : LaboratoryRequestBase, IRequest<ProjectDto?>
{
    public required Guid ProjectId { get; set; }
}

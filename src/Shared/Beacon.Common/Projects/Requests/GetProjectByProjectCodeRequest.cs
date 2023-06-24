using MediatR;

namespace Beacon.Common.Projects.Requests;

public sealed class GetProjectByProjectCodeRequest : LaboratoryRequestBase, IRequest<ProjectDto?>
{ 
    public required ProjectCode ProjectCode { get; set; }
}
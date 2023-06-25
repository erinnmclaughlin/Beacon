using MediatR;

namespace Beacon.Common.Projects.Requests;

public sealed class GetProjectByProjectCodeRequest : IRequest<ProjectDto?>
{ 
    public required ProjectCode ProjectCode { get; set; }
}
using MediatR;

namespace Beacon.Common.Projects.Requests;

public sealed class CompleteProjectRequest : IRequest
{
    public required Guid ProjectId { get; set; }
}

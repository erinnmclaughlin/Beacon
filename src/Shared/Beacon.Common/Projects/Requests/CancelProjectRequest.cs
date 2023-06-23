using MediatR;

namespace Beacon.Common.Projects.Requests;

public sealed class CancelProjectRequest : IRequest
{
    public required Guid ProjectId { get; set; }
}

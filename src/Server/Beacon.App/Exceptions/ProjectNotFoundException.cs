using Beacon.App.ValueObjects;

namespace Beacon.App.Exceptions;

public sealed class ProjectNotFoundException : BeaconException
{
    public ProjectNotFoundException(ProjectCode projectCode) 
        : base(BeaconExceptionType.NotFound, $"Project {projectCode} was not found.")
    {
    }
}

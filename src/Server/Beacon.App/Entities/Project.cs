using Beacon.App.ValueObjects;
using Beacon.Common.Projects;

namespace Beacon.App.Entities;

public class Project : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; set; }
    public required ProjectCode ProjectCode { get; init; }
    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;

    public required Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;
}

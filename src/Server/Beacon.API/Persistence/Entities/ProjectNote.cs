using System;

namespace Beacon.API.Persistence.Entities;

public sealed class ProjectNote : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string Content { get; set; }
    public required DateTime CreatedOn { get; init; } = DateTime.UtcNow;

    public required Guid ProjectId { get; init; }
    public Project Project { get; init; } = null!;

    public required Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;
} 
using System;

namespace Beacon.API.Persistence.Entities;

public class ProjectNote : LaboratoryScopedEntityBase
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedById { get; set; }

    public Project Project { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
} 
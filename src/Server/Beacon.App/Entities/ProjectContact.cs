﻿namespace Beacon.App.Entities;

public sealed class ProjectContact : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required string? EmailAddress { get; set; }
    public required string? PhoneNumber { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}

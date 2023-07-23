﻿namespace Beacon.Common.Models;

public sealed class LaboratoryEventDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required DateTimeOffset ScheduledStart { get; init; }
    public required DateTimeOffset ScheduledEnd { get; init; }
    public required ProjectCode ProjectCode { get; init; }
}
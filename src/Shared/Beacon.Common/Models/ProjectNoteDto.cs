using System;

namespace Beacon.Common.Models;

public sealed record ProjectNoteDto
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid CreatedById { get; init; }
    public required string CreatedByDisplayName { get; init; }
} 
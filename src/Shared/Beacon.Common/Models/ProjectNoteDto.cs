namespace Beacon.Common.Models;

public sealed class ProjectNoteDto
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required LaboratoryMemberDto CreatedBy { get; init; }
} 
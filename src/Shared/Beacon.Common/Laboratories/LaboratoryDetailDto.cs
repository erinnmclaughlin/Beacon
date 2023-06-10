namespace Beacon.Common.Laboratories;

public sealed record LaboratoryDetailDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required  List<LaboratoryMemberDto> Members { get; init; }
}

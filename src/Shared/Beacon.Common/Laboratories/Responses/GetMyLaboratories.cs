using Beacon.Common.Laboratories.Enums;

namespace Beacon.Common.Laboratories.Responses;

public static class GetMyLaboratories
{
    public sealed record Laboratory
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required LaboratoryMember Admin { get; init; }
        public required LaboratoryMembershipType MembershipType { get; init; }
    }

    public sealed record LaboratoryMember
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
    }
}

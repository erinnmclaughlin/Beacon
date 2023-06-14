﻿using Beacon.Common.Laboratories.Enums;

namespace Beacon.Common.Laboratories;

public sealed record LaboratoryDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required LaboratoryMembershipType MyMembershipType { get; init; }
}

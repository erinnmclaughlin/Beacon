using Beacon.Common.Models;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Requests.Projects.SampleGroups;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CreateSampleGroupRequest : IRequest
{
    public string SampleName { get; set; } = "";
    public int? Quantity { get; set; }
    public string? ContainerType { get; set; }
    public double? Volume { get; set; }
    public bool? IsHazardous { get; set; }
    public bool? IsLightSensitive { get; set; }
    public double? TargetStorageTemperature { get; set; }
    public double? TargetStorageHumidity { get; set; }
    public string? Notes { get; set; }

    public required Guid ProjectId { get; set; }

    public sealed class Validator : AbstractValidator<CreateSampleGroupRequest>
    {
        public Validator()
        {
            RuleFor(x => x.SampleName)
                .NotEmpty().WithMessage("Sample name is required");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).When(x => x.Quantity != null).WithMessage("Invalid quantity.");
        }
    }
}

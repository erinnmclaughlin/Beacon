using FluentValidation;

namespace Beacon.Common.Requests.Projects.Events;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Analyst)]
public sealed class CreateProjectEventRequest : BeaconRequest<CreateProjectEventRequest>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set;} = string.Empty;
    public DateTimeOffset ScheduledStart { get; set; }
    public DateTimeOffset ScheduledEnd { get; set; }

    public Guid ProjectId { get; set; }
    public List<Guid> InstrumentIds { get; set; } = new();

    public sealed class Validator : AbstractValidator<CreateProjectEventRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Event title is required.");

            RuleFor(x => x.ScheduledStart)
                .NotEmpty().WithMessage("Start date must be specified.");

            RuleFor(x => x.ScheduledEnd)
                .NotEmpty().WithMessage("End date must be specified.")
                .Must((r, d) => d > r.ScheduledStart).WithMessage("End date cannot be before start date.");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("Project ID must be specified.");
        }
    }
}

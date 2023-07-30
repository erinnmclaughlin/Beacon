using FluentValidation;

namespace Beacon.Common.Requests.Projects.Events;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Analyst)]
public sealed class LinkInstrumentToProjectEventRequest : BeaconRequest<LinkInstrumentToProjectEventRequest>
{
    public Guid ProjectEventId { get; set; }
    public Guid InstrumentId { get; set; }

    public sealed class Validator : AbstractValidator<LinkInstrumentToProjectEventRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectEventId).NotEmpty().WithMessage("Project event must be specified.");
            RuleFor(x => x.InstrumentId).NotEmpty().WithMessage("Instrument must be specified.");
        }
    }
}

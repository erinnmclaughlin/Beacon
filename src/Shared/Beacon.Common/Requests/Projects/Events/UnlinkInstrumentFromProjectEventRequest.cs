using FluentValidation;

namespace Beacon.Common.Requests.Projects.Events;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Analyst)]
public sealed class UnlinkInstrumentFromProjectEventRequest : BeaconRequest<UnlinkInstrumentFromProjectEventRequest>
{
    public Guid ProjectEventId { get; set; }
    public Guid InstrumentId { get; set; }

    public sealed class Validator : AbstractValidator<UnlinkInstrumentFromProjectEventRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectEventId).NotEmpty().WithMessage("Project event must be specified.");
            RuleFor(x => x.InstrumentId).NotEmpty().WithMessage("Instrument must be specified.");
        }
    }
}

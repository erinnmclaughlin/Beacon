using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beacon.Common.Requests.Projects.Events;

public sealed class UnassociateInstrumentFromProjectEventRequest : BeaconRequest<UnassociateInstrumentFromProjectEventRequest>
{
    public Guid ProjectEventId { get; set; }
    public Guid InstrumentId { get; set; }

    public sealed class Validator : AbstractValidator<UnassociateInstrumentFromProjectEventRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectEventId).NotEmpty().WithMessage("Project event must be specified.");
            RuleFor(x => x.InstrumentId).NotEmpty().WithMessage("Instrument must be specified.");
        }
    }
}

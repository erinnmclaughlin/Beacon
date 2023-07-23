using FluentValidation;

namespace Beacon.Common.Requests.Instruments;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Manager)]
public sealed class CreateLaboratoryInstrumentRequest : BeaconRequest<CreateLaboratoryInstrumentRequest>
{
    public string InstrumentType { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;

    public sealed class Validator : AbstractValidator<CreateLaboratoryInstrumentRequest>
    {
        public Validator()
        {
            RuleFor(x => x.InstrumentType)
                .NotEmpty().WithMessage("Instrument type is required.");

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required.");
        }
    }
}

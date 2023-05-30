using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class GetLaboratoryMembershipsByUserIdRequest : IApiRequest<List<LaboratoryMembershipDto>>
{
    public Guid UserId { get; set; }

    public class Validator : AbstractValidator<GetLaboratoryMembershipsByUserIdRequest>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User must be specified.");
        }
    }
}

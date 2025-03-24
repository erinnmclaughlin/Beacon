using Beacon.Common.Models;
using FluentValidation;

namespace Beacon.Common.Requests.Projects.Notes;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class AddProjectNoteRequest : BeaconRequest<AddProjectNoteRequest>
{
    public Guid ProjectId { get; set; }
    public string Content { get; set; } = string.Empty;

    public sealed class Validator : AbstractValidator<AddProjectNoteRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
        }
    }
} 
using System;
using System.Collections.Generic;
using Beacon.Common.Models;
using FluentValidation;

namespace Beacon.Common.Requests.Projects.Notes;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectNotesRequest : BeaconRequest<GetProjectNotesRequest, ProjectNoteDto[]>
{
    public Guid ProjectId { get; init; }

    public sealed class Validator : AbstractValidator<GetProjectNotesRequest>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
} 
using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects.Contacts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects.Contacts;

public sealed class UpdateProjectContact : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapPut("projects/{projectId:guid}/contacts/{contactId:guid}", async (Guid projectId, Guid contactId, UpdateProjectContactRequest request, IMediator m, CancellationToken ct) =>
        {
            if (request.ContactId != contactId || request.ProjectId != projectId)
                return Results.BadRequest();

            await m.Send(request, ct);
            return Results.NoContent();
        });

        builder.WithTags(EndpointTags.Projects);
    }

    internal sealed class Handler : IRequestHandler<UpdateProjectContactRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateProjectContactRequest request, CancellationToken ct)
        {
            var contact = await _dbContext.ProjectContacts
                .SingleAsync(x => x.Id == request.ContactId && x.ProjectId == request.ProjectId, ct);

            contact.Name = request.Name;
            contact.EmailAddress = string.IsNullOrWhiteSpace(request.EmailAddress) ? null : request.EmailAddress;
            contact.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber;

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

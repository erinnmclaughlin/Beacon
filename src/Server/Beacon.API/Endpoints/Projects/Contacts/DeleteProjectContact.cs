using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects.Contacts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects.Contacts;

public sealed class DeleteProjectContact : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapDelete("projects/{projectId:guid}/contacts/{contactId:guid}", async (Guid projectId, Guid contactId, IMediator m, CancellationToken ct) =>
        {
            var request = new DeleteProjectContactRequest { ContactId = contactId, ProjectId = projectId };
            await m.Send(request, ct);
            return Results.NoContent();
        });

        builder.WithTags(EndpointTags.Projects);
    }

    internal sealed class Handler : IRequestHandler<DeleteProjectContactRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DeleteProjectContactRequest request, CancellationToken cancellationToken)
        {
            await _dbContext.ProjectContacts
                .Where(x => x.Id == request.ContactId && x.ProjectId == request.ProjectId)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}

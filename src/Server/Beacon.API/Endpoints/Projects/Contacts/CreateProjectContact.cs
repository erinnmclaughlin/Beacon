using Beacon.API.Endpoints;
using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Requests.Projects.Contacts;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Projects.Contacts;

public sealed class CreateProjectContact : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost("projects/{projectId:guid}/contacts", async (Guid projectId, CreateProjectContactRequest request, IMediator m, CancellationToken ct) =>
        {
            if (request.ProjectId != projectId)
                return Results.BadRequest();

            await m.Send(request, ct);
            return Results.NoContent();
        });

        builder.WithTags(EndpointTags.Projects);
    }

    internal sealed class Handler : IRequestHandler<CreateProjectContactRequest>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Handler(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task Handle(CreateProjectContactRequest request, CancellationToken ct)
        {
            _dbContext.ProjectContacts.Add(new ProjectContact
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                EmailAddress = string.IsNullOrWhiteSpace(request.EmailAddress) ? null : request.EmailAddress,
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber,
                LaboratoryId = _labContext.CurrentLab.Id,
                ProjectId = request.ProjectId
            });

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

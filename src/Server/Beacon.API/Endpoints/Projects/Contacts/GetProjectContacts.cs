using Beacon.API.Endpoints;
using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Contacts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects.Contacts;

public sealed class GetProjectContacts : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("projects/{projectId:Guid}/contacts", async (Guid projectId, IMediator m, CancellationToken ct) =>
        {
            var contacts = await m.Send(new GetProjectContactsRequest { ProjectId = projectId }, ct);
            return Results.Ok(contacts);
        });
    }

    internal sealed class Handler : IRequestHandler<GetProjectContactsRequest, ProjectContactDto[]>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProjectContactDto[]> Handle(GetProjectContactsRequest request, CancellationToken ct)
        {
            return await _dbContext.ProjectContacts
                .Where(x => x.ProjectId == request.ProjectId)
                .Select(x => new ProjectContactDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    EmailAddress = x.EmailAddress,
                    PhoneNumber = x.PhoneNumber
                })
                .ToArrayAsync(ct);
        }
    }
}

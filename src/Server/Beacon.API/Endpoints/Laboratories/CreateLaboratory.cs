using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Laboratories;

public sealed class CreateLaboratory : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<CreateLaboratoryRequest>("laboratories").WithTags(EndpointTags.Laboratories);
    }

    internal sealed class CommandHandler : IRequestHandler<CreateLaboratoryRequest>
    {
        private readonly ISessionContext _context;
        private readonly BeaconDbContext _dbContext;

        public CommandHandler(ISessionContext currentUser, BeaconDbContext dbContext)
        {
            _context = currentUser;
            _dbContext = dbContext;
        }

        public async Task Handle(CreateLaboratoryRequest request, CancellationToken ct)
        {
            var lab = new Laboratory
            {
                Id = Guid.NewGuid(),
                Name = request.LaboratoryName
            };

            lab.AddMember(_context.UserId, LaboratoryMembershipType.Admin);

            _dbContext.Laboratories.Add(lab);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

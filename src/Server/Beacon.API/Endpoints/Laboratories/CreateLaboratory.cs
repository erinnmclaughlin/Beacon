using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common;
using Beacon.Common.Memberships;
using Beacon.Common.Requests.Laboratories;
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
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public CommandHandler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task Handle(CreateLaboratoryRequest request, CancellationToken ct)
        {
            var lab = new Laboratory
            {
                Id = Guid.NewGuid(),
                Name = request.LaboratoryName
            };

            lab.AddMember(_currentUser.UserId, LaboratoryMembershipType.Admin);

            _dbContext.Laboratories.Add(lab);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}

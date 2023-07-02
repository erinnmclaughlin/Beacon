using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects;

public sealed class UpdateLeadAnalyst : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapPut("projects/{projectId:guid}/analyst", async (Guid projectId, UpdateLeadAnalystRequest request, IMediator m, CancellationToken ct) =>
        {
            if (projectId != request.ProjectId)
                return Results.BadRequest();

            await m.Send(request, ct);
            return Results.NoContent();
        });

        builder.WithTags(EndpointTags.Projects);
    }

    public sealed class Validator : AbstractValidator<UpdateLeadAnalystRequest>
    {
        private readonly ILabContext _labContext;

        public Validator(ILabContext labContext)
        {
            _labContext = labContext;

            RuleFor(x => x.AnalystId)
                .MustAsync(BeAuthorized).When(x => x.AnalystId != null).WithMessage("Lead analyst must have at least an analyst role.");
        }

        private async Task<bool> BeAuthorized(Guid? analystId, CancellationToken ct)
        {
            if (analystId == null)
                return true;

            return await _labContext.GetMembershipTypeAsync(analystId.Value, ct) >= LaboratoryMembershipType.Analyst;
        }
    }

    internal sealed class Handler : IRequestHandler<UpdateLeadAnalystRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateLeadAnalystRequest request, CancellationToken ct)
        {
            var project = await _dbContext.Projects.SingleAsync(x => x.Id == request.ProjectId, ct);

            project.LeadAnalystId = request.AnalystId;
            await _dbContext.SaveChangesAsync(ct);                
        }
    }
}

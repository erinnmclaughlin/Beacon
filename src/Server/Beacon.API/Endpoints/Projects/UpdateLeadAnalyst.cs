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
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Validator(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;

            RuleFor(x => x.AnalystId)
                .MustAsync(BeAuthorized)
                .WithMessage("Lead analyst must have at least an analyst role.");
        }

        private async Task<bool> BeAuthorized(Guid? analystId, CancellationToken ct)
        {
            if (analystId == null)
                return true;

            var membership = await _dbContext.Memberships
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.LaboratoryId == _labContext.LaboratoryId && m.MemberId == analystId.Value, ct);

            return membership?.MembershipType is >= LaboratoryMembershipType.Analyst;
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

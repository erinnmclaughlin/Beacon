using Beacon.API.Endpoints;
using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Invitations;

public sealed class AcceptEmailInvitation : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet("invitations/{emailId:Guid}/accept", async (Guid emailId, IMediator m, CancellationToken ct) =>
        {
            await m.Send(new AcceptEmailInvitationRequest { EmailInvitationId = emailId }, ct);
            return Results.NoContent();
        });

        builder.WithTags(EndpointTags.Invitations);
    }

    public sealed class Validator : AbstractValidator<AcceptEmailInvitationRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public Validator(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.EmailInvitationId)
                .MustAsync(NotBeExpired).WithMessage("This invitation has expired.");
        }

        private async Task<bool> NotBeExpired(Guid emailId, CancellationToken ct)
        {
            var emailInvite = await _dbContext.InvitationEmails.AsNoTracking().SingleAsync(i => i.Id == emailId, ct);
            return !emailInvite.IsExpired(DateTimeOffset.UtcNow);
        }
    }

    internal sealed class Handler : IRequestHandler<AcceptEmailInvitationRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task Handle(AcceptEmailInvitationRequest request, CancellationToken ct)
        {
            var currentUser = await GetCurrentUser(ct);
            var invitation = await GetInvitation(request, currentUser.Id, ct);

            invitation.Accept(currentUser);
            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task<User> GetCurrentUser(CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;
            return await _dbContext.Users.SingleAsync(u => u.Id == currentUserId, ct);
        }

        private async Task<Invitation> GetInvitation(AcceptEmailInvitationRequest request, Guid currentUserId, CancellationToken ct)
        {
            return await _dbContext.InvitationEmails
                .Include(i => i.LaboratoryInvitation)
                .ThenInclude(i => i.Laboratory)
                .ThenInclude(l => l.Memberships.Where(m => m.Member.Id == currentUserId))
                .Where(i => i.Id == request.EmailInvitationId)
                .Select(i => i.LaboratoryInvitation)
                .SingleAsync(ct);
        }
    }
}

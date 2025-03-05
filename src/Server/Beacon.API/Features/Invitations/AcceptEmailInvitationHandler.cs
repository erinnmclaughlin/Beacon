using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Services;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Invitations;

internal sealed class AcceptEmailInvitationHandler(ISessionContext context, BeaconDbContext dbContext) : IBeaconRequestHandler<AcceptEmailInvitationRequest>
{
    private readonly ISessionContext _context = context;
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Success>> Handle(AcceptEmailInvitationRequest request, CancellationToken ct)
    {
        var currentUser = await GetCurrentUser(ct);
        var invitation = await GetInvitation(request, currentUser.Id, ct);

        if (invitation.NewMemberEmailAddress != currentUser.EmailAddress)
            return BeaconError.Forbid("The current user's email address does not match the email address in the invitation.");

        if (!invitation.Laboratory.HasMember(currentUser))
        {
            invitation.AcceptedById = currentUser.Id;
            invitation.Laboratory.AddMember(currentUser.Id, invitation.MembershipType);
        }

        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }

    private async Task<User> GetCurrentUser(CancellationToken ct)
    {
        var currentUserId = _context.UserId;
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
            .IgnoreQueryFilters()
            .SingleAsync(ct);
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
            var emailInvite = await _dbContext.InvitationEmails
                .AsNoTracking()
                .IgnoreQueryFilters()
                .SingleAsync(i => i.Id == emailId, ct);

            return !emailInvite.IsExpired(DateTimeOffset.UtcNow);
        }
    }
}

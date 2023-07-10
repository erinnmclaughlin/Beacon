using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.App.Settings;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Beacon.API.Endpoints.Invitations;

public sealed class CreateEmailInvitation : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<CreateEmailInvitationRequest>("invitations").WithTags(EndpointTags.Invitations);
    }

    public sealed class Validator : AbstractValidator<CreateEmailInvitationRequest>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Validator(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;

            RuleFor(r => r.NewMemberEmailAddress)
                .MustAsync(NotBeAnExistingMember)
                .WithMessage("User is already a member of the specified laboratory.");
        }

        private async Task<bool> NotBeAnExistingMember(CreateEmailInvitationRequest request, string email, CancellationToken ct)
        {
            var isAMember = await _dbContext.Memberships
                .AnyAsync(m => m.LaboratoryId == _labContext.CurrentLab.Id && m.Member.EmailAddress == email, ct);

            return !isAMember;
        }
    }

    internal sealed class Handler : IRequestHandler<CreateEmailInvitationRequest>
    {
        private readonly ILabContext _context;
        private readonly BeaconDbContext _dbContext;
        private readonly IPublisher _publisher;

        public Handler(ILabContext context, BeaconDbContext dbContext, IPublisher publisher)
        {
            _context = context;
            _dbContext = dbContext;
            _publisher = publisher;
        }

        public async Task Handle(CreateEmailInvitationRequest request, CancellationToken ct)
        {
            if (_context.CurrentLab?.MembershipType is not { } type || type < request.MembershipType)
                throw new UserNotAllowedException();

            var user = await _dbContext.Users.SingleAsync(x => x.Id == _context.CurrentUser.Id, ct);
            var invitation = await CreateInvitation(request, user, ct);

            await _publisher.Publish(new EmailInvitationCreated(invitation.Id), ct);
        }

        private async Task<InvitationEmail> CreateInvitation(CreateEmailInvitationRequest request, User currentUser, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                CreatedOn = now,
                ExpireAfterDays = 10, // TODO: make this configurable
                NewMemberEmailAddress = request.NewMemberEmailAddress,
                MembershipType = request.MembershipType,
                Laboratory = await _dbContext.Laboratories.SingleAsync(x => x.Id == _context.CurrentLab.Id, ct),
                LaboratoryId = _context.CurrentLab.Id,
                CreatedById = currentUser.Id,
                CreatedBy = currentUser
            };

            var emailInvitation = invitation.AddEmailInvitation(now);

            _dbContext.Invitations.Add(invitation);
            await _dbContext.SaveChangesAsync(ct);

            return emailInvitation;
        }
    }

    public record EmailInvitationCreated(Guid EmailInvitationId) : INotification;

    internal sealed class NotificationHandler : INotificationHandler<EmailInvitationCreated>
    {
        private readonly ApplicationSettings _appSettings;
        private readonly BeaconDbContext _dbContext;
        private readonly IEmailService _emailService;

        public NotificationHandler(IOptions<ApplicationSettings> appSettings, BeaconDbContext dbContext, IEmailService emailService)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public async Task Handle(EmailInvitationCreated notification, CancellationToken ct)
        {
            var emailInvitation = await _dbContext.InvitationEmails
                .Include(i => i.LaboratoryInvitation)
                    .ThenInclude(i => i.CreatedBy)
                .Include(i => i.Laboratory)
                .SingleAsync(i => i.Id == notification.EmailInvitationId, ct);

            var sendOperation = await SendAsync(emailInvitation);

            emailInvitation.OperationId = sendOperation.OperationId;
            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task<IEmailSendOperation> SendAsync(InvitationEmail emailInvitation)
        {
            return await _emailService.SendAsync(
                GetSubject(emailInvitation.LaboratoryInvitation),
                GetBody(_appSettings.BaseUrl, emailInvitation),
                emailInvitation.LaboratoryInvitation.NewMemberEmailAddress)
                ?? throw new Exception("There was an error sending the invitation email."); // TODO: throw better exception
        }

        private static string GetSubject(Invitation invitation)
        {
            return $"{invitation.CreatedBy.DisplayName} invites you to join a lab!";
        }

        private static string GetAcceptUrl(string baseUrl, InvitationEmail invitation)
        {
            return $"{baseUrl}/invitations/{invitation.Id}/accept";
        }

        private static string GetBody(string baseUrl, InvitationEmail emailInvitation)
        {
            var invitation = emailInvitation.LaboratoryInvitation;

            return $"""
            <p>
                <h3>You're invited to join {invitation.Laboratory.Name}!</h3>
                <p>
                    <a href="{GetAcceptUrl(baseUrl, emailInvitation)}">Click here to accept.</a>
                    <br />
                    <small>This invitation will expire in {invitation.ExpireAfterDays} days.</small>
                </p>
            </p>
            """;
        }
    }
}

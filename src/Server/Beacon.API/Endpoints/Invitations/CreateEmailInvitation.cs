using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.App.Settings;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Services;
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
            if (_context.CurrentLab.MembershipType < request.MembershipType)
                throw new UserNotAllowedException();

            await EnsureUserIsNotAnExistingMember(request.NewMemberEmailAddress, ct);

            var invitation = await CreateInvitation(request, ct);

            await _publisher.Publish(new EmailInvitationCreated(invitation.Id), ct);
        }

        private async Task<InvitationEmail> CreateInvitation(CreateEmailInvitationRequest request, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                CreatedOn = now,
                ExpireAfterDays = 10, // TODO: make this configurable
                NewMemberEmailAddress = request.NewMemberEmailAddress,
                MembershipType = request.MembershipType,
                LaboratoryId = _context.CurrentLab.Id,
                CreatedById = _context.CurrentUser.Id
            };

            var emailInvitation = invitation.AddEmailInvitation(now);

            _dbContext.Invitations.Add(invitation);
            await _dbContext.SaveChangesAsync(ct);

            return emailInvitation;
        }

        private async Task EnsureUserIsNotAnExistingMember(string newMemberEmail, CancellationToken ct)
        {
            if (await _dbContext.Memberships.AnyAsync(m => m.LaboratoryId == _context.CurrentLab.Id && m.Member.EmailAddress == newMemberEmail, ct))
                throw new BeaconValidationException(nameof(CreateEmailInvitationRequest.NewMemberEmailAddress), "User is already a member of the specified laboratory.");
        }
    }

    public record EmailInvitationCreated(Guid EmailInvitationId) : INotification;

    internal sealed class NotificationHandler : INotificationHandler<EmailInvitationCreated>
    {
        private readonly string _baseUrl;
        private readonly BeaconDbContext _dbContext;
        private readonly IEmailService _emailService;

        public NotificationHandler(IOptions<ApplicationSettings> appSettings, BeaconDbContext dbContext, IEmailService emailService)
        {
            _baseUrl = appSettings.Value.BaseUrl;
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
                $"{emailInvitation.LaboratoryInvitation.CreatedBy.DisplayName} invites you to join a lab!",
                GetBody($"{_baseUrl}/invitations/{emailInvitation.Id}/accept", emailInvitation.LaboratoryInvitation),
                emailInvitation.LaboratoryInvitation.NewMemberEmailAddress)
                ?? throw new Exception("There was an error sending the invitation email."); // TODO: throw better exception
        }

        private static string GetBody(string acceptUrl, Invitation invitation)
        {
            return $"""
            <p>
                <h3>You're invited to join {invitation.Laboratory.Name}!</h3>
                <p>
                    <a href="{acceptUrl}">Click here to accept.</a>
                    <br />
                    <small>This invitation will expire in {invitation.ExpireAfterDays} days.</small>
                </p>
            </p>
            """;
        }
    }
}

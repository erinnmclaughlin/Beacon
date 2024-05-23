﻿using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.API.Services;
using Beacon.API.Settings;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Services;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Beacon.API.Features.Invitations;

internal sealed class CreateEmailInvitationHandler(ILabContext context, BeaconDbContext dbContext, IPublisher publisher) : IBeaconRequestHandler<CreateEmailInvitationRequest>
{
    private readonly ILabContext _context = context;
    private readonly BeaconDbContext _dbContext = dbContext;
    private readonly IPublisher _publisher = publisher;

    public async Task<ErrorOr<Success>> Handle(CreateEmailInvitationRequest request, CancellationToken ct)
    {
        if (_context.CurrentLab.MembershipType < request.MembershipType)
            return BeaconError.Forbid();

        if (await InvitedUserIsMember(request.NewMemberEmailAddress, ct))
            return BeaconError.Validation(nameof(CreateEmailInvitationRequest.NewMemberEmailAddress), "User is already a member of the specified laboratory.");

        var invitation = await CreateInvitation(request, ct);

        await _publisher.Publish(new EmailInvitationCreated(invitation.Id), ct);
        return Result.Success;
    }

    private async Task<InvitationEmail> CreateInvitation(CreateEmailInvitationRequest request, CancellationToken ct)
    {
        var invitation = new Invitation
        {
            NewMemberEmailAddress = request.NewMemberEmailAddress,
            MembershipType = request.MembershipType,
            CreatedById = _context.CurrentUser.Id
        };

        var email = new InvitationEmail
        {
            ExpiresOn = invitation.CreatedOn.AddDays(invitation.ExpireAfterDays)
        };

        invitation.EmailInvitations.Add(email);

        _dbContext.Invitations.Add(invitation);
        await _dbContext.SaveChangesAsync(ct);

        return email;
    }

    private async Task<bool> InvitedUserIsMember(string newMemberEmail, CancellationToken ct)
    {
        return await _dbContext.Memberships.AnyAsync(m => m.Member.EmailAddress == newMemberEmail, ct);
    }
}

public record EmailInvitationCreated(Guid EmailInvitationId) : INotification;

internal sealed class NotificationHandler(IOptions<ApplicationSettings> appSettings, BeaconDbContext dbContext, IEmailService emailService) : INotificationHandler<EmailInvitationCreated>
{
    private readonly string _baseUrl = appSettings.Value.BaseUrl;
    private readonly BeaconDbContext _dbContext = dbContext;
    private readonly IEmailService _emailService = emailService;

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

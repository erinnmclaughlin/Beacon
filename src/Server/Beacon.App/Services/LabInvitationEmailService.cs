using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Beacon.App.Services;

public sealed class LabInvitationEmailService
{
    private readonly ApplicationSettings _appSettings;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public LabInvitationEmailService(IOptions<ApplicationSettings> appSettings, IEmailService emailService, IUnitOfWork unitOfWork)
    {
        _appSettings = appSettings.Value;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task SendAsync(Guid emailInvitationId, CancellationToken ct)
    {
        var emailInvitation = await _unitOfWork
            .QueryFor<InvitationEmail>(enableChangeTracking: true)
            .Include(l => l.LaboratoryInvitation)
                .ThenInclude(i => i.CreatedBy)
            .Include(l => l.LaboratoryInvitation)
                .ThenInclude(i => i.Laboratory)
            .FirstOrDefaultAsync(l => l.Id == emailInvitationId, ct)
            ?? throw new EmailInvitationNotFoundException(emailInvitationId);

        var emailSendOperation = await _emailService.SendAsync(
            GetSubject(emailInvitation.LaboratoryInvitation),
            GetBody(_appSettings.BaseUrl, emailInvitation),
            emailInvitation.LaboratoryInvitation.NewMemberEmailAddress)
            ?? throw new Exception("There was an error sending the invitation email."); // TODO: throw better exception

        emailInvitation.OperationId = emailSendOperation.OperationId;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static string GetSubject(Invitation invitation)
    {
        return $"{invitation.CreatedBy.DisplayName} invites you to join a lab!";

    }

    private static string GetAcceptUrl(string baseUrl, InvitationEmail invitation)
    {
        return $"{baseUrl}/invitations/{invitation.LaboratoryInvitationId}/accept?emailId={invitation.Id}";
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

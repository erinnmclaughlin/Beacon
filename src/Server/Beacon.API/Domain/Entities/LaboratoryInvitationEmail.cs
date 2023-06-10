namespace Beacon.API.Domain.Entities;

public class LaboratoryInvitationEmail
{
    public required Guid Id { get; init; }
    public required DateTimeOffset SentOn { get; init; }
    public required DateTimeOffset ExpiresOn { get; init; }

    public string? OperationId { get; set; }

    public required Guid LaboratoryInvitationId { get; init; }
    public LaboratoryInvitation LaboratoryInvitation { get; init; } = null!;

    public void Accept(User acceptingUser)
    {
        if (IsExpired(DateTime.UtcNow))
            throw new Exception("This email invitation is expired.");

        LaboratoryInvitation.Accept(acceptingUser);
    }

    public bool IsExpired(DateTimeOffset now)
    {
        return now > ExpiresOn;
    }
}

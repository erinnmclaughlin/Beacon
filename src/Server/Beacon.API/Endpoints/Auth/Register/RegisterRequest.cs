namespace Beacon.API.Endpoints.Auth.Register;

public class RegisterRequest
{
    public required string EmailAddress { get; set; }
    public required string Password { get; set; }
}

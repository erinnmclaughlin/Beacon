using ErrorOr;
using MediatR;

namespace Beacon.Common.Auth.Login;

public class LoginRequest : IApiRequest<UserDto>
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

using Microsoft.AspNetCore.Authentication;

namespace Beacon.API.IntegrationTests;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public Guid UserId { get; set; }
}

using Beacon.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Beacon.API.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    private readonly Guid _userId;

    public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _userId = options.CurrentValue.UserId;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity("Test");
        identity.AddClaim(new(BeaconClaimTypes.UserId, _userId.ToString()));

        var ticket = new AuthenticationTicket(new(identity), "TestScheme");
        var result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }
}

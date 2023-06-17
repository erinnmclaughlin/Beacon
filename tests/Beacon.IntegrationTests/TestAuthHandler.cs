using Beacon.Common.Auth;
using Beacon.Common.Laboratories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Beacon.IntegrationTests;

public class TestAuthSchemeOptions : AuthenticationSchemeOptions
{ 
    public LaboratoryMembershipType? MembershipType { get; set; }
}

public class TestAuthHandler : AuthenticationHandler<TestAuthSchemeOptions>
{
    private readonly LaboratoryMembershipType? _membershipType;

    public TestAuthHandler(IOptionsMonitor<TestAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
        _membershipType = options.CurrentValue.MembershipType;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity("Test");

        identity.AddClaim(BeaconClaimTypes.UserId, TestData.DefaultUser.Id.ToString());

        if (_membershipType?.ToString() is { } membershipType)
        {
            identity.AddClaim(BeaconClaimTypes.LabId, TestData.DefaultLaboratory.Id.ToString());
            identity.AddClaim(BeaconClaimTypes.MembershipType, membershipType);
        }

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
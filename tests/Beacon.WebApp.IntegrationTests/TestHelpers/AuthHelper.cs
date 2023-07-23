using Beacon.Common.Requests.Auth;
using BeaconUI.Core.Common.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Claims;

namespace Beacon.WebApp.IntegrationTests.TestHelpers;

public static class AuthHelper
{
    public static CurrentUser DefaultUser => new()
    {
        Id = new Guid("aeaea2c0-ade9-4af9-a0c1-7f49aff0dc54"),
        DisplayName = "test"
    };

    public static CurrentLab DefaultLab => new()
    {
        Id = Guid.Parse("0fa24c7c-eefb-4909-809d-4b14f0f6f247"),
        Name = "Test Lab",
        MembershipType = LaboratoryMembershipType.Admin
    };

    public static SessionContext DefaultSession => new()
    {
        CurrentUser = DefaultUser,
        CurrentLab = DefaultLab
    };

    public static Claim[] GetClaims(this SessionContext context)
    {
        var claims = context.CurrentUser.GetClaims().AsEnumerable();

        if (context.CurrentLab is { } currentLab)
            claims = claims.Concat(currentLab.GetClaims());

        return claims.ToArray();
    }

    public static void SetNotAuthorized(this BeaconTestContext testContext)
    {
        testContext.Services
            .RemoveAll<IAuthenticationStateNotifier>()
            .AddScoped<IAuthenticationStateNotifier, FakeAuthenticationStateNotifier>();

        testContext.AddTestAuthorization().SetNotAuthorized();
        testContext.MockApi.Fails<GetSessionContextRequest, SessionContext>();
    }

    public static void SetAuthorized(this BeaconTestContext testContext)
        => testContext.SetAuthorized(DefaultSession);

    public static void SetAuthorized(this BeaconTestContext testContext, SessionContext context)
    {
        testContext.Services
            .RemoveAll<IAuthenticationStateNotifier>()
            .AddScoped<IAuthenticationStateNotifier, FakeAuthenticationStateNotifier>();

        testContext
            .AddTestAuthorization()
            .SetAuthorized(context.CurrentUser.DisplayName)
            .SetClaims(context.GetClaims());

        testContext.MockApi.Succeeds<GetSessionContextRequest, SessionContext>(context);
        testContext.MockApi.Succeeds<LogoutRequest>();

    }
}

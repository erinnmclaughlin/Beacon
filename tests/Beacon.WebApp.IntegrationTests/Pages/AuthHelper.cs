using Beacon.Common.Services;

namespace Beacon.WebApp.IntegrationTests.Pages;

public static class AuthHelper
{
    public static SessionContext DefaultSession => new()
    {
        CurrentUser = new()
        {
            Id = new Guid("aeaea2c0-ade9-4af9-a0c1-7f49aff0dc54"),
            DisplayName = "test"
        },
        CurrentLab = null
    };
}

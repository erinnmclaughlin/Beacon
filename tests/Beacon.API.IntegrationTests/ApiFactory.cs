using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<BeaconWebHost>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureBeaconTestServices(); 
        
        builder.ConfigureTestServices(services =>
        {
            services
                .Configure<TestAuthHandlerOptions>(options => options.DefaultUserId = TestData.ManagerUser.Id)
                .AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
        });
    }
}

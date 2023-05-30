using Beacon.API.Auth.Services;
using Beacon.API.Persistence;
using Beacon.Common.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API;

public static class BeaconAPI
{ 
    public static IServiceCollection AddBeaconCore(this IServiceCollection services, Action<DbContextOptionsBuilder> dbOptionsAction)
    {
        services.AddControllers();
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
        services.AddDbContext<BeaconDbContext>(dbOptionsAction);
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ISignInManager, SignInManager>();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(BeaconAPI).Assembly);
        });

        services.AddValidationPipeline();

        return services;
    }
}

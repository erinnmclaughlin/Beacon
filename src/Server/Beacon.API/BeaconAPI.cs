using Beacon.API.Behaviors;
using Beacon.API.Endpoints;
using Beacon.API.Middleware;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.API.Settings;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace Beacon.API;

public static class BeaconAPI
{
    public static IServiceCollection AddBeaconCore(this IServiceCollection services)
    {
        var serviceAssemblies = new[] { typeof(BeaconAPI).Assembly, typeof(LoginRequest).Assembly };

        services.RegisterBeaconRequestHandlers();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(serviceAssemblies);
            config.AddOpenBehavior(typeof(AuthorizationPipelineBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(serviceAssemblies, includeInternalTypes: true);
        services.AddScoped<BeaconAuthenticationService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }

    public static IServiceCollection AddBeaconApi(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> dbOptionsAction)
    {
        services.AddBeaconCore();

        // Auth
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
        });
        services.AddAuthorization();

        // Api
        services.AddEndpointsApiExplorer();
        services.Configure<ApplicationSettings>(config.GetRequiredSection("ApplicationSettings"));
        services.ConfigureHttpJsonOptions(options => {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddHttpContextAccessor();
        services.AddScoped<ISessionContext, HttpSessionContext>();
        services.AddScoped<ILabContext, HttpSessionContext>();
        services.AddScoped<ISignInManager, SignInManager>();

        // Database
        services.AddDbContext<BeaconDbContext>(dbOptionsAction);

        // Email
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<EmailSettings>(config.GetRequiredSection("EmailSettings"));

        return services;
    }

    public static IEndpointRouteBuilder MapBeaconEndpoints<T>(this T app) where T : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = ExceptionHandler.HandleException
        });

        app.MapGroup("api").MapBeaconEndpoints();

        return app;
    }
}

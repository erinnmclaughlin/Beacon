using Beacon.API.App.Services;
using Beacon.API.App.Services.Email;
using Beacon.API.App.Services.Security;
using Beacon.API.App.Settings;
using Beacon.API.Infrastructure;
using Beacon.API.Persistence;
using Beacon.API.Presentation.Endpoints;
using Beacon.API.Presentation.Middleware;
using Beacon.API.Presentation.Services;
using Beacon.Common.Auth.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace Beacon.API;

public static class BeaconAPI
{ 
    public static IServiceCollection AddBeaconCore(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> dbOptionsAction)
    {
        // Api
        services.AddEndpointsApiExplorer().ConfigureHttpJsonOptions(jsonOptions =>
        {
            jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<ApplicationSettings>(config.GetRequiredSection("ApplicationSettings"));

        // Auth
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Data
        services.AddDbContext<BeaconDbContext>(dbOptionsAction);
        services.AddScoped<IUnitOfWork, BeaconDbContext>();
        services.AddScoped<IQueryService, BeaconDbContext>();

        // Email
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<LabInvitationEmailService>();
        services.Configure<EmailSettings>(config.GetRequiredSection("EmailSettings"));

        // Framework
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(BeaconAPI).Assembly);
        });

        services.AddValidatorsFromAssemblies(new[]
        {
            typeof(BeaconAPI).Assembly,
            typeof(LoginRequest).Assembly
        });

        return services;
    }

    public static IEndpointRouteBuilder MapBeaconEndpoints<T>(this T app) where T : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = ExceptionHandler.HandleException
        });

        var endpointRoot = app.MapGroup("api");

        // TODO: register via reflection
        AuthEndpoints.Map(endpointRoot);
        LabEndpoints.Map(endpointRoot);
        UsersEndpoints.Map(endpointRoot);

        return app;
    }
}

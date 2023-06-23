using Beacon.API.Endpoints.Auth;
using Beacon.API.Endpoints.Invitations;
using Beacon.API.Endpoints.Laboratories;
using Beacon.API.Endpoints.Memberships;
using Beacon.API.Endpoints.Projects;
using Beacon.API.Endpoints.Session;
using Beacon.API.Middleware;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Services;
using Beacon.App.Settings;
using Beacon.Common.Auth;
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
    public static IServiceCollection AddBeaconApi(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> dbOptionsAction)
    {
        // Framework:
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(BeaconAPI).Assembly, typeof(LoginRequest).Assembly);
        });

        services.AddValidatorsFromAssemblies(new[]
        {
            typeof(BeaconAPI).Assembly,
            typeof(LoginRequest).Assembly
        });

        // Api
        services.AddEndpointsApiExplorer();
        services.Configure<ApplicationSettings>(config.GetRequiredSection("ApplicationSettings"));
        services.ConfigureHttpJsonOptions(options => {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });


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

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthConstants.LabAuth, config =>
            {
                config.RequireClaim(BeaconClaimTypes.LabId);
            });
        });
        services.AddScoped<BeaconAuthenticationService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, SessionManager>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ISessionManager, SessionManager>();

        // Data
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

        var api = app.MapGroup("api");
        Login.Map(api);
        Logout.Map(api);
        Register.Map(api);
        CreateLaboratory.Map(api);
        GetMyLaboratories.Map(api);
        GetLaboratoryById.Map(api);
        AcceptInvitation.Map(api);
        CreateInvitation.Map(api);
        GetMemberships.Map(api);
        UpdateMembership.Map(api);
        CancelProject.Map(api);
        CompleteProject.Map(api);
        CreateProject.Map(api);
        GetProjects.Map(api);
        GetSessionInfo.Map(api);

        return app;
    }
}

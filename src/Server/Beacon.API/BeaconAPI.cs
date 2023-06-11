﻿using Beacon.API.Endpoints;
using Beacon.API.Infrastructure;
using Beacon.API.Middleware;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App;
using Beacon.App.Services;
using Beacon.App.Settings;
using Beacon.Common.Auth;
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
        services.AddBeaconCore();

        // Api
        services.AddEndpointsApiExplorer().ConfigureHttpJsonOptions(jsonOptions =>
        {
            jsonOptions.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.Configure<ApplicationSettings>(config.GetRequiredSection("ApplicationSettings"));

        // Auth
        services.AddAuthentication().AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiAuth", config =>
            {
                config.RequireClaim(BeaconClaimTypes.LabId);
            });
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, SessionManager>();
        services.AddScoped<ICurrentLab, SessionManager>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ISignInManager, SessionManager>();
        services.AddScoped<ISessionManager, SessionManager>();

        // Data
        services.AddDbContext<BeaconDbContext>(dbOptionsAction);
        services.AddScoped<IUnitOfWork, BeaconDbContext>();
        services.AddScoped<IQueryService, BeaconDbContext>();

        // Email
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<LabInvitationEmailService>();
        services.Configure<EmailSettings>(config.GetRequiredSection("EmailSettings"));

        return services;
    }

    public static IEndpointRouteBuilder MapBeaconEndpoints<T>(this T app) where T : IApplicationBuilder, IEndpointRouteBuilder
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = ExceptionHandler.HandleException
        });

        PortalEndpoints.Map(app);
        ApiEndpoints.Map(app);

        return app;
    }
}

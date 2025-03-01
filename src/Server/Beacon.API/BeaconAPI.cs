﻿using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.API.Settings;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace Beacon.API;

public static class BeaconAPI
{
    public static IServiceCollection AddBeaconApi(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> dbOptionsAction)
    {
        var serviceAssemblies = new[] { typeof(BeaconAPI).Assembly, typeof(LoginRequest).Assembly };

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
        services.AddScoped<BeaconAuthenticationService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Api
        services.AddEndpointsApiExplorer();
        services.Configure<ApplicationSettings>(config.GetRequiredSection("ApplicationSettings"));
        services.ConfigureHttpJsonOptions(o => {
            o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddHttpContextAccessor(); 
        services.RegisterBeaconRequestHandlers();
        services.AddMediatR(c => {
            c.RegisterServicesFromAssemblies(serviceAssemblies);
        });
        services.AddScoped<ISessionContext, HttpSessionContext>();
        services.AddScoped<ILabContext, HttpSessionContext>();
        services.AddScoped<ISignInManager, SignInManager>();

        // Database
        services.AddDbContext<BeaconDbContext>(dbOptionsAction);

        // Email
        services.AddScoped<IEmailService, EmailService>();
        services.Configure<EmailSettings>(config.GetRequiredSection("EmailSettings"));

        // Validation
        services.AddValidatorsFromAssemblies(serviceAssemblies, includeInternalTypes: true);

        return services;
    }
}

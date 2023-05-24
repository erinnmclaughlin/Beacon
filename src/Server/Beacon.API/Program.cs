using Beacon.API.Behaviors;
using Beacon.API.Middleware;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Beacon.Common.Auth.Login;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsSpecs", builder =>
    {
        builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(options => true)
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<BeaconDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServerDb");
    options.UseSqlServer(connectionString);
});

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

builder.Services.AddTransient<ApiExceptionHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("CorsSpecs");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// TODO: this is useful for early development but should be a part of CI/CD later
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<BeaconDbContext>().Database.MigrateAsync();
}

app.UseMiddleware<ApiExceptionHandler>();

app.Run();

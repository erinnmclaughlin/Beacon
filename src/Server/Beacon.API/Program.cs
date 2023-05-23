using Beacon.API.Persistence;
using Beacon.API.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddCors();

builder.Services.AddDbContext<BeaconDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServerDb");
    options.UseSqlServer(connectionString);
});

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.UseCors(o =>  o
    .AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod()
);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// TODO: this is useful for early development but should be a part of CI/CD later
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<BeaconDbContext>().Database.MigrateAsync();
}

app.Run();

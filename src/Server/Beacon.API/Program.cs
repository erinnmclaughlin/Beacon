using Beacon.API.Persistence;
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

app.Run();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(o =>  o
    .AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod()
);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

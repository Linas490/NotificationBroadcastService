using Microsoft.EntityFrameworkCore;
using NotificationApi.Repositories.Interfaces;
using NotificationApi.Repositories.Repositories;
using NotificationApi.Services.Services;
using NotificationApi.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Shared.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, ngsqlOptions =>
        ngsqlOptions.MigrationsAssembly("NotificationApi")));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IActorService, ActorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();      // Swagger support
builder.Services.AddSwaggerGen();                // Swagger generator

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Only run migrations if the DB provider supports it (not supported by in memory databases)
    if (db.Database.IsRelational())
    {
        if (db.Database.GetPendingMigrations().Any())
            db.Database.Migrate();
    }
}
// Enable Swagger in all environments (optional: limit to dev)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification API V1");
    options.RoutePrefix = "swagger"; // This makes Swagger UI the root page 
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

namespace NotificationApi
{
    public partial class Program { }
}

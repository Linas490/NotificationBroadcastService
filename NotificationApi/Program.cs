using Microsoft.EntityFrameworkCore;
using NotificationApi.Data;
using NotificationApi.Repositories.Interfaces;
using NotificationApi.Repositories.Repositories;
using NotificationApi.Services.Services;
using NotificationApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();      // Swagger support
builder.Services.AddSwaggerGen();                // Swagger generator

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

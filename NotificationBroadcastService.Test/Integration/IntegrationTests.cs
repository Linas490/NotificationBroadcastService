using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationApi.Repositories.Interfaces;
using NotificationApi.Repositories.Repositories;
using NotificationTCPServer;
using NotificationTCPServer.Notifiers;
using NotificationTCPServer.Notifiers.AkkaNotifier;
using Shared.Data;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NotificationBroadcastService.Test.Integration
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<NotificationApi.Program>>, IDisposable
    {
        private readonly WebApplicationFactory<NotificationApi.Program> _apiFactory;
        private readonly HttpClient _client;
        private readonly DbContextOptions<AppDbContext> _dbOptions;
        private IHost _tcpHost;

        public IntegrationTests(WebApplicationFactory<NotificationApi.Program> apiFactory)
        {
            // Shared in-memory DB
            _dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            // Web API factory using same in-memory DB
            _apiFactory = apiFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove old context config
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Inject shared in-memory db
                    services.AddSingleton(_dbOptions);
                    services.AddDbContext<AppDbContext>();

                    // Ensure DB is created
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                });
            });

            _client = _apiFactory.CreateClient();

            // Start TCP Server using same db
            _tcpHost = TcpServerHost.StartAsync(_dbOptions).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task Integration_PostNotification_PersistsAndBroadcasts()
        {
            // Arrange: connect fake TCP client
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 5000);
            using var stream = tcpClient.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            string message = "Integration test notification";
            var content = new StringContent($"\"{message}\"", Encoding.UTF8, "application/json");

            // Act: send HTTP POST to API
            var response = await _client.PostAsync("/api/notification", content);

            // Assert API response
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain(message);

            // Assert broadcast received via TCP
            var received = await reader.ReadLineAsync();
            received.Should().Contain(message);

            // Assert persistence
            using var db = new AppDbContext(_dbOptions);
            var saved = await db.Notifications.FirstOrDefaultAsync(n => n.Text == message);
            saved.Should().NotBeNull();
            saved.Text.Should().Be(message);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _tcpHost?.Dispose();
        }

        public static class TcpServerHost
        {
            public static async Task<IHost> StartAsync(DbContextOptions<AppDbContext> dbOptions)
            {
                var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton(dbOptions);
                        services.AddDbContext<AppDbContext>();
                        services.AddScoped<INotificationRepository, NotificationRepository>();

                        services.AddSingleton<NotifierAkka>();
                        services.AddSingleton<INotifier>(sp => sp.GetRequiredService<NotifierAkka>());

                        services.AddSingleton<TCPServer>(sp =>
                        {
                            return new TCPServer(5000, sp.GetRequiredService<INotifier>());
                        });
                    })
                    .Build();

                // Start TCP server (background)
                var server = host.Services.GetRequiredService<TCPServer>();
                _ = server.ListenAsync(); // fire & forget

                return host;
            }
        }
    }
}

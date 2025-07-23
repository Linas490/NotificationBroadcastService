using Akka.Actor;
using NotificationTCPServer;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationTCPServer.Notifiers.AkkaNotifier;
using NotificationApi.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using NotificationApi.Repositories.Interfaces;
using NotificationTCPServer.Notifiers;
using Microsoft.Extensions.Hosting;
using Shared.Data;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddSingleton<NotifierAkka>();
        services.AddSingleton<INotifier>(provider => provider.GetRequiredService<NotifierAkka>());

        services.AddSingleton<TCPServer>(provider =>
        {
            int port = 5000; // Port for TCP server
            var notifier = provider.GetRequiredService<INotifier>();
            return new TCPServer(port, notifier);
        });
    })
    .Build(); 

var server = host.Services.GetRequiredService<TCPServer>();
await server.ListenAsync();


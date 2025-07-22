using Akka.Actor;
using NotificationTCPServer;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationTCPServer.Notifiers.AkkaNotifier;

var notifier = new NotifierAkka();
var server = new TCPServer(5000, notifier);

await server.ListenAsync();

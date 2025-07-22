using Akka.Actor;
using NotificationTCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


var system = ActorSystem.Create("MySystem");
IActorRef notificationActor = system.ActorOf(Props.Create(() => new NotificationActor()), "notificationActor");


int port = 5000;
var server = new TCPServer(port, notificationActor);

await server.ListenAsync();

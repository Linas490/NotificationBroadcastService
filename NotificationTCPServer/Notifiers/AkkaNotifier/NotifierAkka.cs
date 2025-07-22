using Akka.Actor;
using Akka.Configuration;
using NotificationTCPServer.Notifiers.AkkaNotifier.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NotificationTCPServer.Notifiers.AkkaNotifier
{
    public class NotifierAkka : INotifier
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _supervisor;

        public NotifierAkka()
        {
            var config = ConfigurationFactory.ParseString(@"
            akka {
              actor {
                provider = remote
              }
              remote {
                dot-netty.tcp {
                  port = 5001
                  hostname = localhost
                }
              }
            }");

            _actorSystem = ActorSystem.Create("NotificationSystem", config);
            _supervisor = _actorSystem.ActorOf(Props.Create(() => new NotificationSupervisorActor()), "supervisor");
        }

        public void AddClient(TcpClient client)
        {
            _supervisor.Tell(new AddClient(client));
        }

        public void RemoveClient(TcpClient client)
        {
            _supervisor.Tell(new RemoveClient(client));
        }

        public void Notify(string message)
        {
            _supervisor.Tell(new Notification(message));
        }
    }
}

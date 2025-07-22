using Akka.Actor;
using Akka.Configuration;
using NotificationApi.Services.Interfaces;
using System.Net.Sockets;
using static Shared.MessageTypes;

namespace NotificationApi.Services.Services
{
    public class ActorService : IActorService
    {
        private readonly ActorSystem system;
        private readonly ActorSelection remoteActor;

        public record Notification(string Message);

        public ActorService()
        {
            var config = ConfigurationFactory.ParseString(@"
                akka {
                    actor {
                        provider = remote
                    }
                    remote {
                        dot-netty.tcp {
                        port = 0
                        hostname = localhost
                        }
                    }
                    }");

            system = ActorSystem.Create("RemoteClientSystem", config);
            remoteActor = system.ActorSelection("akka.tcp://NotificationSystem@localhost:5001/user/supervisor");
        }

        public Task SendNotificationAsync(string message)
        {
            remoteActor.Tell(new Notify(message));
            return Task.CompletedTask;
        }
    }
}

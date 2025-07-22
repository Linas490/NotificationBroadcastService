using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using NotificationTCPServer.Notifiers.AkkaNotifier.Actors;
using Shared;
using System.Net.Sockets;
using static Shared.MessageTypes;

namespace NotificationTCPServer.Notifiers.AkkaNotifier
{
    public class NotifierAkka : INotifier
    {
        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _supervisor;

        public NotifierAkka(IServiceProvider serviceProvider)
        {
            // Akka konfigūracija (galima vėliau iškelti)
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

            // Sukuriam DI setup'ą Akka
            var di = DependencyResolverSetup.Create(serviceProvider);
            var bootstrap = BootstrapSetup.Create().WithConfig(config);
            var setup = bootstrap.And(di);

            _actorSystem = ActorSystem.Create("NotificationSystem", setup);

            // Registruojam resolver
            var resolver = DependencyResolver.For(_actorSystem);

            // Sukuriam supervisor per DI
            _supervisor = _actorSystem.ActorOf(resolver.Props<NotificationSupervisorActor>(), "supervisor");
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
            _supervisor.Tell(new Notify(message));
        }

        public async Task ShutdownAsync()
        {
            await _actorSystem.Terminate();
        }
    }
}

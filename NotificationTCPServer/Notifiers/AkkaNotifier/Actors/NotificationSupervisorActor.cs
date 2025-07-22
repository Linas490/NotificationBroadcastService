using Akka.Actor;
using Akka.DependencyInjection;
using Shared;
using static Shared.MessageTypes;

namespace NotificationTCPServer.Notifiers.AkkaNotifier.Actors
{
    internal class NotificationSupervisorActor : ReceiveActor
    {
        private IActorRef _notificationActor;

        public NotificationSupervisorActor()
        {
            var resolver = DependencyResolver.For(Context.System);
            _notificationActor = Context.ActorOf(resolver.Props<NotificationActor>(), "notification");

            Receive<AddClient>(msg => _notificationActor.Tell(msg));
            Receive<RemoveClient>(msg => _notificationActor.Tell(msg));
            Receive<NotifyPersist>(msg => _notificationActor.Tell(msg));
            Receive<Notify>(msg => _notificationActor.Tell(msg));
        }
    }
}

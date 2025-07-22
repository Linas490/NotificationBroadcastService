using Akka.Actor;
using Notiffcations.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationTCPServer.Notifiers.AkkaNotifier.Actors
{
    internal class NotificationSupervisorActor : ReceiveActor
    {
        private IActorRef _notificationActor;

        public NotificationSupervisorActor()
        {
            _notificationActor = Context.ActorOf(Props.Create(() => new NotificationActor()), "notification");

            Receive<AddClient>(msg => _notificationActor.Tell(msg));
            Receive<RemoveClient>(msg => _notificationActor.Tell(msg));
            Receive<Notification>(msg => _notificationActor.Tell(msg));
        }
    }
}

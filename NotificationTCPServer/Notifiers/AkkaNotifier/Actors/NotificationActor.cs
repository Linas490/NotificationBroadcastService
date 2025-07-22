using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Notiffcations.Shared;

namespace NotificationTCPServer.Notifiers.AkkaNotifier.Actors
{
    public class NotificationActor : ReceiveActor
    {
        private readonly HashSet<TcpClient> _clients = new HashSet<TcpClient>();
        public NotificationActor()
        {
            Receive<AddClient>(msg =>
            {
                _clients.Add(msg.Client);
            });

            Receive<RemoveClient>(msg =>
            {
                _clients.Remove(msg.Client);
            });

            Receive<Notification>(msg =>
            {
                var data = Encoding.UTF8.GetBytes(msg.Message + "\n");

                foreach (var client in _clients)
                {
                    try
                    {
                        if (client.Connected)
                        {
                            client.GetStream().Write(data, 0, data.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        _clients.Remove(client);
                    }
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NotificationTCPServer.Notifiers
{
    public interface INotifier
    {
        void AddClient(TcpClient client);
        void RemoveClient(TcpClient client);
        void Notify(string message);
    }
}

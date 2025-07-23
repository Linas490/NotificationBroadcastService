using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class MessageTypes
    {
        //message types of actor
        public record Notify(string Message);
        public record NotifyPersist(string Message);
        public record AddClient(TcpClient Client);
        public record RemoveClient(TcpClient Client);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationTCPServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int port = 5000;
            var server = new TCPServer(port);

            await server.ListenAsync();
        }
    }
}

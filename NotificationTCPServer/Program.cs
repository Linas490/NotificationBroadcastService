using NotificationTCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

int port = 5000;
var server = new TCPServer(port);

await server.ListenAsync();

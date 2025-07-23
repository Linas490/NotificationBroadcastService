using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Akka.Actor;
using NotificationTCPServer.Notifiers;

namespace NotificationTCPServer
{
    public class TCPServer
    {
        private TcpListener _listener;
        private INotifier _notifier;

        public TCPServer(int port, INotifier notifier)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _notifier = notifier;
        }

        public async Task ListenAsync() 
        {
            //we listen for incoming TCP connections and hadle them (get stream-messages)
            _listener.Start();
            Console.WriteLine("TCP SERVER MESSAGE: Server started. Waiting for connections...");

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("TCP SERVER MESSAGE: Client connected.");
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                // Add client to notifier - will pass to notifier actor
                _notifier.AddClient(client);

                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    ProcessLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP SERVER ERROR: {ex.Message}");
            }
            finally
            {
                //when stream is closed, we remove client from notifier
                client.Close();
                _notifier.RemoveClient(client);
                Console.WriteLine("TCP SERVER MESSAGE: Client disconnected.");
            }
        }

        private void ProcessLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line.StartsWith("NOTIFY:", StringComparison.OrdinalIgnoreCase))
            {
                string message = line.Substring("NOTIFY:".Length).Trim();

                if (string.IsNullOrEmpty(message))
                {
                    Console.WriteLine("TCP SERVER WARNING: Empty NOTIFY message received.");
                    return;
                }

                Console.WriteLine("TCP SERVER MESSAGE: Received notification: " + message);

                //Send to actor
                _notifier.NotifyPersist(message);
            }
            else
            {
                Console.WriteLine("TCP SERVER MESSAGE: Invalid message received: " + line);
            }
        }

    }
}

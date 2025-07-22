using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NotificationTCPServer
{
    public class TCPServer
    {
        private TcpListener _listener;

        public TCPServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task ListenAsync() 
        {
            _listener.Start();
            Console.WriteLine("TCP SERVER MESSAGE: Server started. Waiting for connections...");

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("TCP SERVER MESSAGE: Client connected.");
                HandleClientAsync(client);
            }
        }

        private async void HandleClientAsync(TcpClient client)
        {
            try
            {
                //add client to actor
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[1024];
                    int bytesRead;
                    var sb = new StringBuilder();

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                        // Check for newline - we process line by line
                        string content = sb.ToString();
                        int newlineIndex;

                        while ((newlineIndex = content.IndexOf('\n')) >= 0)
                        {
                            string line = content.Substring(0, newlineIndex).Trim('\r', '\n');
                            content = content.Substring(newlineIndex + 1);

                            // Process the line
                            ProcessLine(line);

                            sb.Clear();
                            sb.Append(content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TCP SERVER ERROR: {ex.Message}");
            }
            finally
            {
                client.Close();
                //Remove client from actor
                Console.WriteLine("TCP SERVER MESSAGE: Client disconnected.");
            }
        }
        private void ProcessLine(string line)
        {
            if (line.StartsWith("NOTIFY:", StringComparison.OrdinalIgnoreCase))
            {
                string message = line.Substring("NOTIFY:".Length).Trim();
                Console.WriteLine("TCP SERVER MESSAGE: Received notification: " + message);
                //Send to actor
            }
            else
            {
                Console.WriteLine("TCP SERVER MESSAGE: Invalid message received: " + line);
            }
        }
    }
}

using Akka.Actor;
using NotificationApi.Repositories.Interfaces;
using System.Net.Sockets;
using System.Text;
using static Shared.MessageTypes;

public class NotificationActor : ReceiveActor
{
    private readonly HashSet<TcpClient> _clients = new HashSet<TcpClient>();
    private readonly INotificationRepository _notificationRepository;
    UTF8Encoding utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    public NotificationActor(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;

        Receive<AddClient>(msg =>
        {
            _clients.Add(msg.Client);
        });

        Receive<RemoveClient>(msg =>
        {
            _clients.Remove(msg.Client);
        });

        Receive<Notify>(msg =>
        {
            var data = Encoding.UTF8.GetBytes(msg.Message.Trim() + "\n");

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

        ReceiveAsync<NotifyPersist>(async msg =>
        {
            try
            {
                await _notificationRepository.AddNotificationAsync(new()
                {
                    Text = msg.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB insert error: {ex.Message}");
            }

            var data = Encoding.UTF8.GetBytes(msg.Message.Trim() + "\n");

            foreach (var client in _clients.ToList()) // copy list to avoid modification during iteration
            {
                try
                {
                    if (client.Connected)
                    {

                        using var writer = new StreamWriter(client.GetStream(), utf8NoBom, leaveOpen: true);
                        await writer.WriteLineAsync(msg.Message.Trim());
                        await writer.FlushAsync();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"TCP write error: {ex.Message}");
                    _clients.Remove(client);
                }
            }
        });
    }
}

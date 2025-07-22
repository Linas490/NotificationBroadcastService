using Akka.Actor;
using NotificationApi.Repositories.Interfaces;
using System.Net.Sockets;
using System.Text;
using static Shared.MessageTypes;

public class NotificationActor : ReceiveActor
{
    private readonly HashSet<TcpClient> _clients = new HashSet<TcpClient>();
    private readonly INotificationRepository _notificationRepository;

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

        ReceiveAsync<Notify>(async msg =>
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

            var data = Encoding.UTF8.GetBytes(msg.Message + "\n");

            foreach (var client in _clients.ToList()) // copy list to avoid modification during iteration
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
                    Console.WriteLine($"TCP write error: {ex.Message}");
                    _clients.Remove(client);
                }
            }
        });
    }
}

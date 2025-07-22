using System.Net.Sockets;

namespace Notiffcations.Shared
{
    public record Notification(string Message);
    public record AddClient(TcpClient Client);
    public record RemoveClient(TcpClient Client);
}

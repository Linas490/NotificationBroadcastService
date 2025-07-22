using System.Net.Sockets;

namespace Notiffcations.Shared
{
    public record Notify(string Message);
    public record AddClient(TcpClient Client);
    public record RemoveClient(TcpClient Client);
}

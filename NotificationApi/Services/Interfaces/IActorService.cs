namespace NotificationApi.Services.Interfaces
{
    public interface IActorService
    {
        Task SendNotificationAsync(string message);
    }
}

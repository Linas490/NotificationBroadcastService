using NotificationApi.Models;

namespace NotificationApi.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<bool> AddNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetLastNotificationsAsync(int limit);
    }
}

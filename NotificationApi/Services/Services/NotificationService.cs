using NotificationApi.Models;
using NotificationApi.Repositories.Interfaces;
using NotificationApi.Services.Interfaces;

namespace NotificationApi.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository) => this._notificationRepository = notificationRepository;

        public async Task<bool> AddNotificationAsync(Notification notification)
        {
            if ((notification == null) || string.IsNullOrWhiteSpace(notification.Text))
                return false;

            return await _notificationRepository.AddNotificationAsync(notification);
            //Will Call actor/actorservice later
        }

        public async Task<IEnumerable<Notification>> GetLastNotificationsAsync(int limit)
        {
            return await _notificationRepository.GetLastNotificationsAsync(limit);
        }
    }
}

using NotificationApi.Models;
using NotificationApi.Repositories.Interfaces;
using NotificationApi.Services.Interfaces;

namespace NotificationApi.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IActorService _actorService;
        public NotificationService(INotificationRepository notificationRepository, IActorService actorService) 
        {
            _notificationRepository = notificationRepository;
            _actorService = actorService;
        }

        public async Task<bool> AddNotificationAsync(Notification notification)
        {
            if ((notification == null) || string.IsNullOrWhiteSpace(notification.Text))
                return false;

            _actorService.SendNotificationAsync(notification.Text);
            return await _notificationRepository.AddNotificationAsync(notification);
        }

        public async Task<IEnumerable<Notification>> GetLastNotificationsAsync(int limit)
        {
            return await _notificationRepository.GetLastNotificationsAsync(limit);
        }
    }
}

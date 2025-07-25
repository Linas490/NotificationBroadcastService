﻿using Shared.Model;

namespace NotificationApi.Services.Interfaces
{
    public interface INotificationService
    {
        Task<bool> AddNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetLastNotificationsAsync(int limit);
    }
}

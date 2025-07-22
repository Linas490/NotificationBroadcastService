using Microsoft.EntityFrameworkCore;
using NotificationApi.Data;
using NotificationApi.Models;
using NotificationApi.Repositories.Interfaces;
using System.Collections.Generic;

namespace NotificationApi.Repositories.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context) => this._context = context;

        public async Task<bool> AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<IEnumerable<Notification>> GetLastNotificationsAsync(int limit)
        {
            return await _context.Notifications
                .OrderByDescending(n => n.Timestamp)
                .Take(limit)
                .ToListAsync();
        }
    }
}
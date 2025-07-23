using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NotificationApi.Repositories.Repositories;
using Shared.Data;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationBroadcastService.Test.Repositories
{
    public class NotificationRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly NotificationRepository _repository;

        public NotificationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new AppDbContext(options);
            _repository = new NotificationRepository(_context);
        }

        [Fact]
        public async Task NotificationRepository_AddNotificationAsync_ShouldAddNotification() 
        {
            // Arrange
            var notification = new Shared.Model.Notification
            {
                Text = "Test Notification",
                Timestamp = DateTime.UtcNow
            };

            // Act
            var result = await _repository.AddNotificationAsync(notification);

            // Assert
            result.Should().BeTrue();
            _context.Notifications.Should().ContainSingle(n => n.Text == "Test Notification");
        }

        [Fact]
        public async Task NotificationRepository_GetLastNotificationsAsync_ShouldReturnNotificationsOrderedByTimestampDescending()
        {
            // Arrange
            var oldNotification = new Notification { Text = "Old", Timestamp = DateTime.UtcNow.AddHours(-2) };
            var newNotification = new Notification { Text = "New", Timestamp = DateTime.UtcNow };
            await _context.Notifications.AddRangeAsync(oldNotification, newNotification);
            await _context.SaveChangesAsync();

            // Act
            var lastNotifications = await _repository.GetLastNotificationsAsync(2);

            // Assert
            lastNotifications.Should().HaveCount(2);
            lastNotifications.First().Text.Should().Be("New");
            lastNotifications.Last().Text.Should().Be("Old");
        }

        [Fact]
        public async Task NotificationRepository_GetLastNotificationsAsync_ShouldReturnLimitedNumberOfNotifications()
        {
            int addImagesNum = 5;
            int getImagesNum = addImagesNum + 2; // Requesting more than added to test limit

            // Arrange
            for (int i = 0; i < addImagesNum; i++)
            {
                await _context.Notifications.AddAsync(new Notification
                {
                    Text = $"Notification {i}",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i)
                });
            }
            await _context.SaveChangesAsync();


            // Act
            var lastNotifications = await _repository.GetLastNotificationsAsync(addImagesNum);

            // Assert
            lastNotifications.Should().HaveCount(addImagesNum);
        }
    }
}

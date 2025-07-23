using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NotificationApi.Controllers;
using NotificationApi.Services.Interfaces;
using Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NotificationBroadcastService.Test.Controllers
{
    public class NotificationControllerTests
    {
        private readonly INotificationService _notificationService;
        private readonly NotificationController _controller;

        public NotificationControllerTests()
        {
            _notificationService = A.Fake<INotificationService>();
            _controller = new NotificationController(_notificationService);
        }

        [Fact]
        public async Task NotificationController_GetLastNotificationsAsync_ShouldReturnBadRequest_WhenLimitIsNegative()
        {
            // Act
            var result = await _controller.GetLastNotificationsAsync(-1);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<string>()
                .Which.Should().Contain("FAILED");
        }

        [Fact]
        public async Task NotificationController_GetLastNotificationsAsync_ShouldReturnOk_WhenLimitIsValid()
        {
            // Arrange
            var limit = 2;
            var fakeNotifications = A.Fake<ICollection<Notification>>();
            A.CallTo(() => _notificationService.GetLastNotificationsAsync(limit)).Returns(fakeNotifications);

            // Act
            var result = await _controller.GetLastNotificationsAsync(limit);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeSameAs(fakeNotifications);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public async Task NotificationController_AddNotificationAsync_ShouldReturnBadRequest_WhenTextIsInvalid(string text)
        {
            // Act
            var result = await _controller.AddNotificationAsync(text);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<string>()
                .Which.Should().Contain("FAILED");
        }

        [Fact]
        public async Task NotificationController_AddNotificationAsync_ShouldReturnBadRequest_WhenTextIsTooLong()
        {
            // Arrange
            var longText = new string('x', 501);

            // Act
            var result = await _controller.AddNotificationAsync(longText);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("FAILED: Message too long.");
        }

        [Fact]
        public async Task NotificationController_AddNotificationAsync_ShouldReturnOk_WhenTextIsValid()
        {
            // Arrange
            var validText = "Test message";

            A.CallTo(() => _notificationService.AddNotificationAsync(
                A<Notification>.That.Matches(n => n.Text == validText)))
                .Returns(true);

            // Act
            var result = await _controller.AddNotificationAsync(validText);

            // Assert
            A.CallTo(() => _notificationService.AddNotificationAsync(
                A<Notification>.That.Matches(n => n.Text == validText)))
                .MustHaveHappenedOnceExactly();

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<Notification>()
                .Which.Text.Should().Be(validText);
        }

    }
}

using System;
using System.Threading.Tasks;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task CreateNotificationAsync(string title, string message, string type, string severity, Guid? userId = null, string targetRole = "All", string brandName = "GreenPantry")
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                Severity = severity,
                UserId = userId,
                TargetRole = targetRole,
                BrandName = brandName,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                TenantId = null // Notifications are global or cross-tenant for now, or use mapped tenant if needed
            };

            await _notificationRepository.CreateAsync(notification);
        }
    }
}

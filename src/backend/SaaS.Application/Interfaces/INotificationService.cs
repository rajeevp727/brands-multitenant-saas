using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaaS.Application.DTOs;
using SaaS.Domain.Entities;

namespace SaaS.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(Notification notification);
        Task<List<NotificationDto>> GetNotificationsAsync(Guid userId, string userRole, Guid? tenantId, bool isSuperAdmin);
        Task MarkAsReadAsync(Guid notificationId);
        Task<int> GetUnreadCountAsync(Guid userId, string userRole, Guid? tenantId);
    }
}

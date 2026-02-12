using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaaS.Application.DTOs;
using SaaS.Application.Interfaces;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationDto> CreateNotificationAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return MapToDto(notification);
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId, string userRole, Guid? tenantId, bool isSuperAdmin)
        {
            var query = _context.Notifications.AsQueryable();

            // 1. Tenant Filter
            if (isSuperAdmin)
            {
                // Super Admin can see ALL notifications (Global View)
                // Optionally, if they passed a specific tenantId to filter by, we respect it
                if (tenantId.HasValue)
                {
                    query = query.Where(n => n.TenantId == tenantId.Value);
                }
            }
            else
            {
                // Regular User: MUST only see their own tenant's notifications OR Global System notifications (TenantId == null)
                query = query.Where(n => n.TenantId == tenantId || n.TenantId == null);
            }

            // 2. Role/User Filter
            // Users should see notifications targeted to 'All', their 'Role', or their specific 'UserId'
            // Super Admin generally sees everything, but let's stick to the requirement "View notifications from ALL brands"
            // For Super Admin dashboard, we might want to see EVERYTHING.
            if (!isSuperAdmin)
            {
                query = query.Where(n => 
                    n.TargetRole == "All" ||
                    n.TargetRole == userRole || 
                    n.UserId == userId);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50) // Limit for performance
                .ToListAsync();

            return notifications.Select(MapToDto).ToList();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, string userRole, Guid? tenantId)
        {
             // Simplified count logic matching GetNotifications filters roughly
             var query = _context.Notifications.AsNoTracking();
             
             if (tenantId.HasValue)
                query = query.Where(n => n.TenantId == tenantId || n.TenantId == null);
             else
                query = query.Where(n => n.TenantId == null); // Fallback

             query = query.Where(n => 
                    (n.TargetRole == "All" || n.TargetRole == userRole || n.UserId == userId) && 
                    !n.IsRead);

             return await query.CountAsync();
        }

        private static NotificationDto MapToDto(Notification n)
        {
            return new NotificationDto
            {
                Id = n.Id,
                TenantId = n.TenantId,
                BrandName = n.BrandName,
                TargetRole = n.TargetRole,
                Type = n.Type,
                Severity = n.Severity,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                TimeAgo = GetTimeAgo(n.CreatedAt)
            };
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            return $"{(int)span.TotalDays}d ago";
        }
    }
}

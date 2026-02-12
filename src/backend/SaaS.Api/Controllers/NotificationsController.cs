using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Application.DTOs;
using SaaS.Application.Interfaces;
using SaaS.Domain.Entities;
using System.Security.Claims;

namespace SaaS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();
            var tenantId = GetCurrentTenantId();
            var isSuperAdmin = IsSuperAdmin();

            var notifications = await _notificationService.GetNotificationsAsync(userId, userRole, tenantId, isSuperAdmin);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();
            var tenantId = GetCurrentTenantId();

            var count = await _notificationService.GetUnreadCountAsync(userId, userRole, tenantId);
            return Ok(count);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")] // Adjust roles as needed
        public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                TenantId = request.TenantId,
                BrandName = request.BrandName,
                TargetRole = request.TargetRole, // "All", "Admin", etc.
                Type = request.Type,
                Severity = request.Severity,
                Title = request.Title,
                Message = request.Message,
                UserId = request.UserId
            };

            var created = await _notificationService.CreateNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotifications), new { id = created.Id }, created);
        }

        // Helper methods to extract claims
        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim != null ? Guid.Parse(claim) : Guid.Empty;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }

        private Guid? GetCurrentTenantId()
        {
            var claim = User.FindFirst("tenantid")?.Value; // Standardize claim name
            return claim != null ? Guid.Parse(claim) : null;
        }

        private bool IsSuperAdmin()
        {
            // Logic to determine super admin. E.g. specific Role or TenantId
            // For Rajeev's Pvt Ltd, let's assume TenantId matching the parent or specific Role "SuperAdmin"
            var role = GetCurrentUserRole();
            return role == "SuperAdmin";
        }
    }

    public class CreateNotificationRequest
    {
        public Guid? TenantId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string TargetRole { get; set; } = "All";
        public string Type { get; set; } = "Info";
        public string Severity { get; set; } = "Info";
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
    }
}

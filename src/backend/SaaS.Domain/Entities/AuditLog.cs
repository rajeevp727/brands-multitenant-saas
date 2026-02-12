using System;
using SaaS.Domain.Common;

namespace SaaS.Domain.Entities;

public class AuditLog : ITenantEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? Changes { get; set; } // JSON of changed properties
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? RemoteIpAddress { get; set; }
}

using Microsoft.EntityFrameworkCore;
using SaaS.Domain.Common;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace SaaS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    private readonly IUserContext _userContext;
    public string? CurrentTenantId => _tenantProvider.GetTenantId();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider, IUserContext userContext)
        : base(options)
    {
        _tenantProvider = tenantProvider;
        _userContext = userContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();
    public DbSet<AppConstant> AppConstants => Set<AppConstant>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ... existing configurations ...
        modelBuilder.Entity<AuditLog>().HasIndex(a => a.TenantId);
        modelBuilder.Entity<AuditLog>().HasIndex(a => a.UserId);
        modelBuilder.Entity<AuditLog>().HasIndex(a => a.Timestamp);

        // Many-to-Many: Role <=> Permission
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "RolePermissions",
                j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"));

        // Indexing Strategy: Every tenant-specific table should have an index on TenantId
        modelBuilder.Entity<Brand>().HasIndex(b => b.TenantId);
        modelBuilder.Entity<Product>().HasIndex(p => p.TenantId);
        modelBuilder.Entity<Order>().HasIndex(o => new { o.TenantId, o.CreatedAt });
        modelBuilder.Entity<Product>().HasIndex(p => new { p.TenantId, p.Name });
        modelBuilder.Entity<User>().HasIndex(u => new { u.TenantId, u.RoleId });
        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.TenantId, n.IsRead });
        modelBuilder.Entity<Notification>().HasIndex(n => n.TenantId);
        modelBuilder.Entity<Role>().HasIndex(r => r.TenantId);

        // Unique Email per Tenant
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Email, u.TenantId })
            .IsUnique();

        // Optimize lookup for Username per Tenant
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Username, u.TenantId })
            .IsUnique();

        // Custom filter for AppConstant (Global or Tenant Specific)
        modelBuilder.Entity<AppConstant>()
    .HasQueryFilter(e => e.TenantId == GetCurrentTenantId() || e.TenantId == null);

        // Apply Global Query Filter for Multi-Tenancy
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(CreateTenantFilterExpression(entityType.ClrType));
            }
        }
    }

    private LambdaExpression CreateTenantFilterExpression(Type type)
    {
        var parameter = Expression.Parameter(type, "e");

        var tenantProperty = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(string) },
            parameter,
            Expression.Constant(nameof(ITenantEntity.TenantId)));

        var getTenantMethod = typeof(ApplicationDbContext)
            .GetMethod(nameof(GetCurrentTenantId), BindingFlags.Instance | BindingFlags.NonPublic)!;

        var tenantIdCall = Expression.Call(Expression.Constant(this), getTenantMethod);

        var body = Expression.Equal(tenantProperty, tenantIdCall);

        return Expression.Lambda(body, parameter);
    }

    private string GetCurrentTenantId()
    {
        return _tenantProvider.GetTenantId()!;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new Exception("Tenant context missing. Middleware not executed.");

        // -----------------------------
        // HARD TENANT SECURITY ENFORCEMENT
        // -----------------------------
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Always overwrite tenant coming from API payload
                    entry.Entity.TenantId = tenantId;
                    break;

                case EntityState.Modified:
                    // Prevent tenant switching attack
                    entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
                    break;

                case EntityState.Deleted:
                    // Prevent deleting another tenant's record
                    if (entry.Entity.TenantId != tenantId)
                        throw new UnauthorizedAccessException("Cross-tenant delete attempt blocked.");
                    break;
            }
        }

        // -----------------------------
        // AUDIT CREATION (BEFORE SAVE → single DB transaction)
        // -----------------------------
        ChangeTracker.DetectChanges();

        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog ||
                entry.State == EntityState.Detached ||
                entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TenantId = tenantId,
                UserId = _userContext.UserId ?? "System",
                EntityName = entry.Entity.GetType().Name
            };

            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                var propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = "Create";
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = "Delete";
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.AuditType = "Update";
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }

        // Attach audit logs BEFORE save (same transaction)
        foreach (var auditEntry in auditEntries)
            AuditLogs.Add(auditEntry.ToAudit());

        // -----------------------------
        // SINGLE DATABASE COMMIT
        // -----------------------------
        return await base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableThreadSafetyChecks(false);
    }
}

internal class AuditEntry
{
    public AuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        Entry = entry;
    }
    public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; }
    public string UserId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string AuditType { get; set; } = string.Empty;
    public Dictionary<string, object?> KeyValues { get; } = new();
    public Dictionary<string, object?> OldValues { get; } = new();
    public Dictionary<string, object?> NewValues { get; } = new();
    public List<string> ChangedColumns { get; } = new();

    public AuditLog ToAudit()
    {
        var audit = new AuditLog();
        audit.UserId = UserId;
        audit.TenantId = TenantId;
        audit.Action = AuditType;
        audit.EntityName = EntityName;
        audit.Timestamp = DateTime.UtcNow;
        audit.EntityId = System.Text.Json.JsonSerializer.Serialize(KeyValues);
        audit.Changes = NewValues.Count == 0 ? null : System.Text.Json.JsonSerializer.Serialize(new { old = OldValues, @new = NewValues });
        return audit;
    }
}

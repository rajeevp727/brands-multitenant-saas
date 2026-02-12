using Microsoft.EntityFrameworkCore;
using SaaS.Domain.Common;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;

namespace SaaS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    public string? CurrentTenantId => _tenantProvider.GetTenantId();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
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
        modelBuilder.Entity<Order>().HasIndex(o => o.TenantId);
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
        modelBuilder.Entity<AppConstant>().HasQueryFilter(e => e.TenantId == CurrentTenantId || e.TenantId == null);

        // Apply Global Query Filter for Multi-Tenancy
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(CreateTenantFilterExpression(entityType.ClrType));
            }
        }
    }

    private System.Linq.Expressions.LambdaExpression CreateTenantFilterExpression(Type type)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantEntity.TenantId));
        
        var dbContext = System.Linq.Expressions.Expression.Constant(this);
        var currentTenantIdProperty = typeof(ApplicationDbContext).GetProperty(nameof(CurrentTenantId));
        var tenantIdCall = System.Linq.Expressions.Expression.Property(dbContext, currentTenantIdProperty!);
        
        var tenantCheck = System.Linq.Expressions.Expression.Equal(property, tenantIdCall);
        return System.Linq.Expressions.Expression.Lambda(tenantCheck, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetTenantId();
        var auditEntries = OnBeforeSaveChanges(tenantId);

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (string.IsNullOrEmpty(entry.Entity.TenantId))
                    {
                        entry.Entity.TenantId = tenantId ?? throw new Exception("TenantId is required for new entities.");
                    }
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<AuditableEntity<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries, cancellationToken);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges(string? tenantId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry)
            {
                TenantId = tenantId ?? "Global",
                UserId = CurrentTenantId ?? "System", // Fallback to tenantId if userId not easily accessible here
                EntityName = entry.Entity.GetType().Name
            };
            auditEntries.Add(auditEntry);

            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
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
        return auditEntries;
    }

    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAudit());
        }
        return base.SaveChangesAsync(cancellationToken);
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

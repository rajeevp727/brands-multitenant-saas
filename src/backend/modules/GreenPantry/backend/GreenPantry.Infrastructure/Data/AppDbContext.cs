using GreenPantry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace GreenPantry.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema for GreenPantry entities
        modelBuilder.HasDefaultSchema("greenpantry");

        // Configure User - explicitly map to public schema to share with SaaS core
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "public", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Id);
        });

        // Configure Notification - explicitly map to public schema
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications", "public", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Id);
        });

        // Configure Restaurant
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(e => e.Address);
            entity.Property(e => e.DeliveryFee).HasColumnType("decimal(18,2)");
            
            entity.Property(e => e.CuisineTypes)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("text")
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c == null ? new List<string>() : c.ToList()));

            entity.Property(e => e.ImageUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("text")
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c == null ? new List<string>() : c.ToList()));
        });

        // Configure MenuItem
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            
            entity.OwnsMany(e => e.Variants, b => 
            {
                b.ToJson();
                b.Property(v => v.PriceModifier).HasColumnType("decimal(18,2)");
            });
        });

        // Configure Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DeliveryFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");

            entity.OwnsOne(e => e.DeliveryAddress);
            
            entity.OwnsMany(e => e.Items, b => 
            {
                b.ToJson();
                b.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                b.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            });
            entity.OwnsMany(e => e.StatusHistory, b => b.ToJson());
            
            entity.HasMany(e => e.Payments)
                  .WithOne()
                  .HasForeignKey(p => p.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(18,2)");
            
            entity.OwnsMany(e => e.Webhooks, b => 
            {
                b.ToJson();
                b.Ignore(w => w.ParsedPayload);
            });

            // Map Dictionary to JSON string
            entity.Property(e => e.ProviderMetadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>())
                .HasColumnType("text")
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, object>>(
                    (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.Count == c2.Count && !c1.Except(c2).Any()),
                    c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
                    c => c == null ? new Dictionary<string, object>() : new Dictionary<string, object>(c)));
        });
    }
}

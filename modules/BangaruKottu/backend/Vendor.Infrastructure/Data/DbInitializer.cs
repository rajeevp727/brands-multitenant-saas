using Microsoft.EntityFrameworkCore;
using Vendor.Domain.Entities;

namespace Vendor.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        if (await context.Products.AnyAsync())
        {
            return; // DB has been seeded
        }

        // ONE: SEED CATEGORIES
        var categories = new List<Category>
        {
            new Category { Name = "Gold Jewelry", Description = "22K and 24K pure gold ornaments" },
            new Category { Name = "Diamond Jewelry", Description = "Certified natural diamond collections" },
            new Category { Name = "Silver Articles", Description = "Pure silver puja items and jewelry" },
            new Category { Name = "Gemstones", Description = "Precious and semi-precious stones" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // TWO: SEED VENDORS (If Vendor entity exists, assuming simple link for now or just Products)
        // Since VendorEntity logic might be complex, let's focus on Products for the "Marketplace" feel.

        // THREE: SEED PRODUCTS
        var goldCat = categories.First(c => c.Name == "Gold Jewelry");
        var diamondCat = categories.First(c => c.Name == "Diamond Jewelry");

        var products = new List<Product>
        {
            new Product
            {
                Name = "22K Gold Antique Necklace",
                Description = "Handcrafted antique finish gold necklace with intricate temple design. Weight: 45g",
                Price = 285000,
                StockQuantity = 5,
                CategoryId = goldCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1611591437281-460bfbe1220a?w=800",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Diamond Solitaire Ring",
                Description = "1 Carat VVS1 Clarity Natural Diamond Ring in 18K White Gold setting.",
                Price = 145000,
                StockQuantity = 10,
                CategoryId = diamondCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1605100804763-247f67b3557e?w=800",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Traditional Gold Bangles Set",
                Description = "Set of 4 refined 22K gold bangles. Total Weight: 60g",
                Price = 380000,
                StockQuantity = 3,
                CategoryId = goldCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1573408301185-9146fe634ad0?w=800",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Bridal Diamond Choker",
                Description = "Exquisite diamond choker necklace for weddings. 15 cts total diamond weight.",
                Price = 1250000,
                StockQuantity = 1,
                CategoryId = diamondCat.Id,
                ImageUrl = "https://images.unsplash.com/photo-1599643478518-17488fbbcd75?w=800",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}

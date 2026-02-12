using GreenPantry.Domain.Entities;
using GreenPantry.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO;

namespace GreenPantry.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Force cleanup of the schema to resolve migration conflicts
        // This is necessary because of the recent migration reset and partial state on the DB.
        await context.Database.ExecuteSqlRawAsync("DROP SCHEMA IF EXISTS greenpantry CASCADE;");
        await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA IF NOT EXISTS greenpantry;");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" LIKE '%GP%';");

        // Apply migrations
        await context.Database.MigrateAsync();

        // Seed Users
        var usersPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "users.json");
        if (!File.Exists(usersPath))
        {
            usersPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "GreenPantry.Infrastructure", "Data", "SeedData", "users.json");
        }

        if (File.Exists(usersPath))
        {
            var usersJson = await File.ReadAllTextAsync(usersPath);
            var userData = JsonSerializer.Deserialize<List<SeedUser>>(usersJson);
            if (userData != null)
            {
                foreach (var userDto in userData)
                {
                    var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
                    if (existingUser == null)
                    {
                        var user = new User
                        {
                            Username = userDto.Username,
                            FirstName = userDto.FirstName,
                            LastName = userDto.LastName,
                            Email = userDto.Email,
                            PhoneNumber = userDto.PhoneNumber,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                            Role = (UserRole)userDto.Role,
                            TenantId = userDto.TenantId,
                            IsActive = userDto.IsActive,
                            IsEmailVerified = userDto.IsEmailVerified,
                            StreetAddress = userDto.StreetAddress,
                            City = userDto.City,
                            State = userDto.State,
                            PostalCode = userDto.PostalCode,
                            Country = userDto.Country,
                            CreatedBy = "System"
                        };
                        await context.Users.AddAsync(user);
                    }
                    else
                    {
                        existingUser.Username = userDto.Username;
                        existingUser.FirstName = userDto.FirstName;
                        existingUser.LastName = userDto.LastName;
                        existingUser.Role = (UserRole)userDto.Role;
                        existingUser.TenantId = userDto.TenantId;
                        existingUser.IsActive = userDto.IsActive;
                        existingUser.CreatedBy = "System";
                        context.Users.Update(existingUser);
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        // Seed Restaurants
        var restaurantsPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "restaurants.json");
        if (!File.Exists(restaurantsPath))
        {
            restaurantsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "GreenPantry.Infrastructure", "Data", "SeedData", "restaurants.json");
        }

        if (File.Exists(restaurantsPath))
        {
            var restaurantsJson = await File.ReadAllTextAsync(restaurantsPath);
            var restaurantData = JsonSerializer.Deserialize<List<SeedRestaurant>>(restaurantsJson);
            if (restaurantData != null)
            {
                foreach (var restDto in restaurantData)
                {
                    var existing = await context.Restaurants.FirstOrDefaultAsync(r => r.Name == restDto.Name);
                    if (existing == null)
                    {
                        var owner = await context.Users.FirstOrDefaultAsync(u => u.Email == restDto.OwnerEmail);
                        var restaurant = new Restaurant
                        {
                            Name = restDto.Name,
                            Description = restDto.Description,
                            OwnerId = owner?.Id ?? Guid.Empty,
                            CuisineTypes = restDto.CuisineTypes,
                            Address = restDto.Address,
                            IsActive = restDto.IsActive,
                            Rating = restDto.Rating,
                            ReviewCount = restDto.ReviewCount,
                            EstimatedDeliveryTime = restDto.EstimatedDeliveryTime,
                            DeliveryFee = restDto.DeliveryFee,
                            ImageUrl = restDto.ImageUrl,
                            ImageUrls = new List<string>(),
                            CreatedBy = "System"
                        };
                        await context.Restaurants.AddAsync(restaurant);
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        // Seed Menu Items
        var menuPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "menu.json");
        if (!File.Exists(menuPath))
        {
            menuPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "GreenPantry.Infrastructure", "Data", "SeedData", "menu.json");
        }

        if (File.Exists(menuPath))
        {
            var menuJson = await File.ReadAllTextAsync(menuPath);
            var menuData = JsonSerializer.Deserialize<List<SeedMenu>>(menuJson);
            if (menuData != null)
            {
                foreach (var menuEntry in menuData)
                {
                    var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Name.ToLower().Replace(" ", "-") == menuEntry.RestaurantSlug);
                    if (restaurant != null)
                    {
                        foreach (var itemDto in menuEntry.Items)
                        {
                            var existingItem = await context.MenuItems.FirstOrDefaultAsync(m => m.RestaurantId == restaurant.Id && m.Name == itemDto.Name);
                            if (existingItem == null)
                            {
                                var menuItem = new MenuItem
                                {
                                    RestaurantId = restaurant.Id,
                                    Name = itemDto.Name,
                                    Description = itemDto.Description,
                                    Price = itemDto.Price,
                                    Category = itemDto.Category,
                                    IsVegetarian = itemDto.IsVegetarian,
                                    IsVegan = itemDto.IsVegan,
                                    IsAvailable = itemDto.IsAvailable,
                                    ImageUrl = itemDto.ImageUrl,
                                    Allergens = new List<string>(),
                                    Ingredients = new List<string>(),
                                    Tags = new List<string>(),
                                    CreatedBy = "System"
                                };
                                await context.MenuItems.AddAsync(menuItem);
                            }
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }

    private class SeedUser
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Role { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    private class SeedRestaurant
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public List<string> CuisineTypes { get; set; } = new();
        public Address Address { get; set; } = new();
        public bool IsActive { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int EstimatedDeliveryTime { get; set; }
        public decimal DeliveryFee { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    private class SeedMenu
    {
        public string RestaurantSlug { get; set; } = string.Empty;
        public List<SeedMenuItem> Items { get; set; } = new();
    }

    private class SeedMenuItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsVegetarian { get; set; }
        public bool IsVegan { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}

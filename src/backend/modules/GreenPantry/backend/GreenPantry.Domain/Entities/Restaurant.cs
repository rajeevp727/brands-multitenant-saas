using GreenPantry.Domain.Enums;

namespace GreenPantry.Domain.Entities;

public class Restaurant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty; // keeping for flat access if needed, or remove? I'll keep it but Address object has City too.
    public string State { get; set; } = string.Empty;
    public Address? Address { get; set; }
    // public string PostalCode { get; set; } // Removing as Address has it
    // Latitude/Longitude are in Address too but Restaurant has them explicitly. I'll keep them for now.
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> CuisineTypes { get; set; } = new(); // Changed to List<string> for simplicity in SQL or keeping List<CuisineType> enum? 
    // The previous file had List<CuisineType>. I should check CuisineType enum.
    // DbInitializer tried "Healthy" string.
    public double Rating { get; set; } = 0.0;
    public int ReviewCount { get; set; } = 0;
    public decimal DeliveryFee { get; set; } = 0;
    public int EstimatedDeliveryTime { get; set; } = 30; // minutes
    public bool IsActive { get; set; } = true;
    public Guid OwnerId { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public RestaurantStatus Status { get; set; } = RestaurantStatus.Pending;
}


using Vendor.Application.DTOs.Product;

namespace Vendor.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetVendorProductsAsync(int vendorId);
    Task<ProductDto?> GetProductByIdAsync(int productId, int vendorId);
    Task<ProductDto> CreateProductAsync(int vendorId, CreateProductRequest request);
    Task<ProductDto?> UpdateProductAsync(int productId, int vendorId, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int productId, int vendorId);
    Task<bool> ToggleProductStatusAsync(int productId, int vendorId);
}


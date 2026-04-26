using AutoMapper;
using Vendor.Application.DTOs.Product;
using Vendor.Application.Interfaces;
using Vendor.Domain.Entities;

namespace Vendor.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IRepository<Category> categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetVendorProductsAsync(int vendorId)
    {
        var products = await _productRepository.GetByVendorIdAsync(vendorId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId, int vendorId)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(productId);
        if (product == null || product.VendorId != vendorId)
            return null;

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(int vendorId, CreateProductRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        var product = new Product
        {
            VendorId = vendorId,
            CategoryId = request.CategoryId,
            ProductName = request.ProductName,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ImageUrl = request.ImageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.AddAsync(product);
        var productWithDetails = await _productRepository.GetProductWithDetailsAsync(createdProduct.ProductId);
        return _mapper.Map<ProductDto>(productWithDetails!);
    }

    public async Task<ProductDto?> UpdateProductAsync(int productId, int vendorId, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null || product.VendorId != vendorId)
            return null;

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null)
            throw new ArgumentException("Category not found");

        product.CategoryId = request.CategoryId;
        product.ProductName = request.ProductName;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.ImageUrl = request.ImageUrl;

        await _productRepository.UpdateAsync(product);

        var updatedProduct = await _productRepository.GetProductWithDetailsAsync(productId);
        return _mapper.Map<ProductDto>(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(int productId, int vendorId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null || product.VendorId != vendorId)
            return false;

        await _productRepository.DeleteAsync(product);
        return true;
    }

    public async Task<bool> ToggleProductStatusAsync(int productId, int vendorId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null || product.VendorId != vendorId)
            return false;

        product.IsActive = !product.IsActive;
        await _productRepository.UpdateAsync(product);
        return true;
    }
}


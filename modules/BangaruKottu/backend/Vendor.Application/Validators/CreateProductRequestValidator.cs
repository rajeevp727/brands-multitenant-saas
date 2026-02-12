using FluentValidation;
using Vendor.Application.DTOs.Product;

namespace Vendor.Application.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(255).WithMessage("Product name must not exceed 255 characters");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category is required");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).When(x => x.StockQuantity.HasValue)
            .WithMessage("Stock quantity cannot be negative");
    }
}


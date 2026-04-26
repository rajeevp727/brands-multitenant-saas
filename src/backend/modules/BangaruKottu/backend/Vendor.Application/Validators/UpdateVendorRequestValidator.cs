using FluentValidation;
using Vendor.Application.DTOs.Vendor;

namespace Vendor.Application.Validators;

public class UpdateVendorRequestValidator : AbstractValidator<UpdateVendorRequest>
{
    public UpdateVendorRequestValidator()
    {
        RuleFor(x => x.VendorName)
            .NotEmpty().WithMessage("Vendor name is required")
            .MaximumLength(255).WithMessage("Vendor name must not exceed 255 characters");

        RuleFor(x => x.ContactNumber)
            .MaximumLength(20).WithMessage("Contact number must not exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.ContactNumber));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Address));
    }
}


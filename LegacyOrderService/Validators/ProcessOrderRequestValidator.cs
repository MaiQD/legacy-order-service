using FluentValidation;
using LegacyOrderService.Models;

namespace LegacyOrderService.Validators;

public class ProcessOrderRequestValidator : AbstractValidator<ProcessOrderRequest>
{
    public ProcessOrderRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name cannot be empty")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name cannot be empty")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
    }
}


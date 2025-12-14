using Application.Features.Carts.DTOs;
using Application.Features.Products.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Carts.Validators;

public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartDtoValidator(IProductRepository productRepository)
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.GreaterThanZero, "ProductId"))
            .MustAsync(async (id, ct) => await productRepository.GetByIdAsync(id) != null)
            .WithMessage(ErrorMessages.ProductNotFound);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.GreaterThanZero, "quantity"))
            .LessThanOrEqualTo(99).WithMessage(string.Format(ErrorMessages.MaxQuantity, 99));
    }
}


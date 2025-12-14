using Application.Features.Carts.DTOs;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Carts.Validators;

public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.GreaterThanZero, "quantity"))
            .LessThanOrEqualTo(99).WithMessage(string.Format(ErrorMessages.MaxQuantity, 99));
    }
}


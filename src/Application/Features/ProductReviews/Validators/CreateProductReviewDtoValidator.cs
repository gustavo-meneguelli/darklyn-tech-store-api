using Application.Features.ProductReviews.DTOs;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.ProductReviews.Validators;

public class CreateProductReviewDtoValidator : AbstractValidator<CreateProductReviewDto>
{
    public CreateProductReviewDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.InvalidId, "product"));

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage(string.Format(ErrorMessages.RangeValue, "rating", 1, 5));

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "comment"))
            .MinimumLength(10).WithMessage(string.Format(ErrorMessages.MinLength, "comment", 10))
            .MaximumLength(1000).WithMessage(string.Format(ErrorMessages.MaxLength, "comment", 1000));
    }
}

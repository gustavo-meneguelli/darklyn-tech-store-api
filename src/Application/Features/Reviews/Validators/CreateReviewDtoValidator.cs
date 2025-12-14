using Application.Features.Reviews.DTOs;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Reviews.Validators;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.InvalidId, "order"));

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage(string.Format(ErrorMessages.RangeValue, "rating", 1, 5));

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "comment"))
            .MinimumLength(10).WithMessage(string.Format(ErrorMessages.MinLength, "comment", 10))
            .MaximumLength(500).WithMessage(string.Format(ErrorMessages.MaxLength, "comment", 500));
    }
}


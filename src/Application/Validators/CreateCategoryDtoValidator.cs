using Application.DTO.Categories;
using FluentValidation;

namespace Application.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be greater than 3 characters.")
            .MaximumLength(50).WithMessage("Name must be less than 50 characters.");
    }
}
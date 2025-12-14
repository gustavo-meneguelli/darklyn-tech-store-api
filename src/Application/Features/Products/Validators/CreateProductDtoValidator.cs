using Application.Features.Products.DTOs;
using Application.Features.Products.Repositories;
using Application.Features.Categories.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Products.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        // Validação de Name: sync primeiro, async só se passar nas básicas
        RuleFor(p => p.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "name"))
            .MinimumLength(3).WithMessage(string.Format(ErrorMessages.MinLength, "name", 3))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "name", 50))
            .MustAsync(async (name, _) =>
            {
                bool exists = await productRepository.ExistByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "product", "name"));

        // Validação de Price: apenas sync
        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.GreaterThanZero, "price"))
            .LessThan(100000).WithMessage(string.Format(ErrorMessages.MaxValue, "price", 100000));

        // Validação de CategoryId: sync primeiro, async só se ID válido
        RuleFor(p => p.CategoryId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.InvalidId, "category"))
            .MustAsync(async (id, _) =>
            {
                var category = await categoryRepository.GetByIdAsync(id);
                return category is not null;
            })
            .WithMessage(ErrorMessages.CategoryNotFound);

        // Validação de Description: obrigatória
        RuleFor(p => p.Description)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "description"))
            .MinimumLength(10).WithMessage(string.Format(ErrorMessages.MinLength, "description", 10))
            .MaximumLength(2000).WithMessage(string.Format(ErrorMessages.MaxLength, "description", 2000));

        // Validação de ImageUrl: obrigatória, formato URL
        RuleFor(p => p.ImageUrl)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "image URL"))
            .MaximumLength(500).WithMessage(string.Format(ErrorMessages.MaxLength, "image URL", 500))
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(string.Format(ErrorMessages.InvalidUrl, "image URL"));
    }
}


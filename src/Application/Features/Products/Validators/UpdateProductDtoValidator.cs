using Application.Features.Products.DTOs;
using Application.Features.Products.Repositories;
using Application.Features.Categories.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Products.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        // Validação de Name: update parcial, só valida se informado
        RuleFor(p => p.Name)
            .Cascade(CascadeMode.Stop)
            .MinimumLength(3).WithMessage(string.Format(ErrorMessages.MinLength, "name", 3))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "name", 50))
            .MustAsync(async (name, _) =>
            {
                bool exists = await productRepository.ExistByNameAsync(name);
                return !exists;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "product", "name"))
            .When(p => p.Name != string.Empty);

        // Validação de Price: update parcial, só valida se informado
        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.GreaterThanZero, "price"))
            .LessThan(100000).WithMessage(string.Format(ErrorMessages.MaxValue, "price", 100000))
            .When(p => p.Price != 0);

        // Validação de CategoryId: update parcial, só valida se informado
        RuleFor(p => p.CategoryId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage(string.Format(ErrorMessages.InvalidId, "category"))
            .MustAsync(async (id, _) =>
            {
                var category = await categoryRepository.GetByIdAsync(id);
                return category is not null;
            })
            .WithMessage(ErrorMessages.CategoryNotFound)
            .When(p => p.CategoryId != 0);

        // Validação de Description: update parcial, só valida se informado
        RuleFor(p => p.Description)
            .MinimumLength(10).WithMessage(string.Format(ErrorMessages.MinLength, "description", 10))
            .MaximumLength(2000).WithMessage(string.Format(ErrorMessages.MaxLength, "description", 2000))
            .When(p => p.Description != string.Empty);

        // Validação de ImageUrl: update parcial, só valida se informado
        RuleFor(p => p.ImageUrl)
            .MaximumLength(500).WithMessage(string.Format(ErrorMessages.MaxLength, "image URL", 500))
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(string.Format(ErrorMessages.InvalidUrl, "image URL"))
            .When(p => p.ImageUrl != string.Empty);
    }
}


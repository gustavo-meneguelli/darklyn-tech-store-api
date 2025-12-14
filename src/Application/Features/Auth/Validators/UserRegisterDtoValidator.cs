using Application.Features.Auth.DTOs;
using Application.Features.Auth.Repositories;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Auth.Validators;

public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator(IUserRepository userRepository)
    {
        // Validação de Username: sync primeiro, async só se passar nas básicas
        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "username"))
            .MinimumLength(3).WithMessage(string.Format(ErrorMessages.MinLength, "username", 3))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "username", 50))
            .MustAsync(async (username, _) =>
            {
                var user = await userRepository.GetUserByUsernameAsync(username);
                return user is null;
            })
            .WithMessage(string.Format(ErrorMessages.AlreadyExists, "user", "username"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "password"))
            .MinimumLength(6).WithMessage(string.Format(ErrorMessages.MinLength, "password", 6))
            .MaximumLength(100).WithMessage(string.Format(ErrorMessages.MaxLength, "password", 100));

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "first name"))
            .MinimumLength(2).WithMessage(string.Format(ErrorMessages.MinLength, "first name", 2))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "first name", 50));

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "last name"))
            .MinimumLength(2).WithMessage(string.Format(ErrorMessages.MinLength, "last name", 2))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "last name", 50));
    }
}

using Application.Features.Auth.DTOs;
using Domain.Constants;
using FluentValidation;

namespace Application.Features.Auth.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "username"))
            .MinimumLength(3).WithMessage(string.Format(ErrorMessages.MinLength, "username", 3))
            .MaximumLength(50).WithMessage(string.Format(ErrorMessages.MaxLength, "username", 50));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(string.Format(ErrorMessages.RequiredField, "password"))
            .MinimumLength(6).WithMessage(string.Format(ErrorMessages.MinLength, "password", 6));
    }
}

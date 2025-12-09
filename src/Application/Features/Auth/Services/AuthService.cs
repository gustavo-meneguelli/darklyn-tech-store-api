using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Auth.DTOs;
using Application.Features.Auth.Repositories;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Auth.Services;

public class AuthService(IUserRepository userRepository, IPasswordHash passwordHash, ITokenService tokenService, IUnitOfWork unitOfWork)
    : IAuthService
{
    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await userRepository.GetUserByUsernameAsync(dto.Username);

        if (user is null)
        {
            return Result<AuthResponseDto>.Unauthorized(string.Format(ErrorMessages.CredentialsInvalid));
        }

        var passwordIsValid = passwordHash.VerifyHashedPassword(dto.Password, user.PasswordHash);

        if (!passwordIsValid)
        {
            return Result<AuthResponseDto>.Unauthorized(string.Format(ErrorMessages.CredentialsInvalid));
        }

        var authResponse = tokenService.GenerateAuthResponse(user);

        return Result<AuthResponseDto>.Success(authResponse);
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(UserRegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash.HashPassword(dto.Password),
            Role = UserRole.Common
        };

        await userRepository.AddAsync(user);

        await unitOfWork.CommitAsync();

        var authResponse = tokenService.GenerateAuthResponse(user);

        return Result<AuthResponseDto>.Success(authResponse);
    }
}



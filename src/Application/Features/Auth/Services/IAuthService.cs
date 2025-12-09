using Application.Common.Models;
using Application.Features.Auth.DTOs;

namespace Application.Features.Auth.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto login);
    Task<Result<AuthResponseDto>> RegisterAsync(UserRegisterDto dto);
}


